using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using AuctionPortal.ApplicationLayer.IApplication;
using AuctionPortal.Common.Auth;
using AuctionPortal.Common.Controllers;
using AuctionPortal.Common.Core;
using AuctionPortal.Models;
using AuctionPortal.Hubs;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace AuctionPortal.Controllers
{
    [Authorize] // require JWT like FavouritesController
    public class AuctionBidsController : APIBaseController
    {
        #region Constructor

        /// <summary>
        /// AuctionBidsController initializes class object.
        /// </summary>
        public AuctionBidsController(
            IAuctionBidApplication auctionBidApplication,
            IInventoryAuctionApplication inventoryAuctionApplication,
            IAuctionApplication auctionApplication,
            INotificationApplication notificationApplication,
            IHubContext<NotificationHub> hub,
            IHeaderValue headerValue,
            IConfiguration configuration)
            : base(headerValue, configuration)
        {
            _auctionBidApplication = auctionBidApplication;
            _inventoryAuctionApp = inventoryAuctionApplication;
            _auctionApplication = auctionApplication;
            _notificationApplication = notificationApplication;
            _hub = hub;
        }

        #endregion

        #region Fields / Properties

        private readonly IAuctionBidApplication _auctionBidApplication;
        private readonly IInventoryAuctionApplication _inventoryAuctionApp;
        private readonly IAuctionApplication _auctionApplication;
        private readonly INotificationApplication _notificationApplication;
        private readonly IHubContext<NotificationHub> _hub;

        #endregion

        #region Helpers

        private int GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimsConstants.UserIdClaimType)
                        ?? User.FindFirst(ClaimTypes.NameIdentifier);

            return claim != null && int.TryParse(claim.Value, out var id) ? id : 0;
        }

        /// <summary>
        /// Persist a Notification row in the DB and push it over SignalR.
        /// </summary>
        private async Task CreateAndPushNotificationAsync(
            int targetUserId,
            string type,
            string title,
            string message,
            int auctionId,
            int inventoryAuctionId,
            int createdByUserId)
        {
            var notification = new Notification
            {
                UserId = targetUserId,
                Type = type,
                Title = title,
                Message = message,
                IsRead = false,
                CreatedById = createdByUserId,
                AuctionId = auctionId,
                InventoryAuctionId = inventoryAuctionId
            };

            await _notificationApplication.Add(notification);

            await _hub.Clients
                .User(targetUserId.ToString())
                .SendAsync("NotificationCreated", notification);
        }

        /// <summary>
        /// Builds bid-related notifications and:
        ///   - sends "bid-outbid" to previous winner (if any)
        ///   - sends "bid-winning" to current bidder (if winning)
        /// Includes dedupe using NotificationApplication.
        /// </summary>
        private async Task SendBidNotificationsAsync(
            AuctionBid newBid,
            AuctionBid? previousWinningBid)
        {
            if (newBid.InventoryAuctionId <= 0)
                return;

            // Load lot & auction info for nice titles
            var invAuc = await _inventoryAuctionApp.Get(new InventoryAuction
            {
                InventoryAuctionId = newBid.InventoryAuctionId
            });

            if (invAuc == null)
                return;

            var timebox = await _auctionApplication.GetTimebox(new Auction
            {
                AuctionId = invAuc.AuctionId
            });

            var auctionName = timebox?.AuctionName ?? "Auction";
            var titleBase = $"{auctionName} — Lot #{invAuc.InventoryId}";

            var status = newBid.AuctionBidStatusName?.Trim() ?? string.Empty;

            // 1) Notify previous winner that they have been outbid
            if (previousWinningBid != null &&
                previousWinningBid.CreatedById.HasValue &&
                previousWinningBid.CreatedById.Value != (newBid.CreatedById ?? 0) &&
                status.Equals("Winning", StringComparison.OrdinalIgnoreCase))
            {
                var oldWinnerId = previousWinningBid.CreatedById.Value;

                // Dedupe – if there's already an outbid notification for this lot/user, don't spam
                var existing = await _notificationApplication.GetForUser(oldWinnerId, unreadOnly: false, top: 200);
                bool alreadyHasOutbid = existing.Any(n =>
                    string.Equals(n.Type, "bid-outbid", StringComparison.OrdinalIgnoreCase) &&
                    n.AuctionId == invAuc.AuctionId &&
                    n.InventoryAuctionId == invAuc.InventoryAuctionId);

                if (!alreadyHasOutbid)
                {
                    var msg = $"You have been outbid on {titleBase}. " +
                              $"Your previous bid of {previousWinningBid.BidAmount:N2} is no longer winning.";

                    await CreateAndPushNotificationAsync(
                        targetUserId: oldWinnerId,
                        type: "bid-outbid",
                        title: $"{titleBase} — You were outbid",
                        message: msg,
                        auctionId: invAuc.AuctionId,
                        inventoryAuctionId: invAuc.InventoryAuctionId,
                        createdByUserId: newBid.CreatedById ?? oldWinnerId);
                }
            }

            // 2) Notify current bidder that their bid is currently winning
            if (status.Equals("Winning", StringComparison.OrdinalIgnoreCase) &&
                newBid.CreatedById.HasValue)
            {
                var currentBidderId = newBid.CreatedById.Value;

                var existing = await _notificationApplication.GetForUser(currentBidderId, unreadOnly: false, top: 200);
                bool alreadyHasWinning = existing.Any(n =>
                    string.Equals(n.Type, "bid-winning", StringComparison.OrdinalIgnoreCase) &&
                    n.AuctionId == invAuc.AuctionId &&
                    n.InventoryAuctionId == invAuc.InventoryAuctionId);

                if (!alreadyHasWinning)
                {
                    var msg = $"Your bid of {newBid.BidAmount:N2} is currently winning for {titleBase}.";

                    await CreateAndPushNotificationAsync(
                        targetUserId: currentBidderId,
                        type: "bid-winning",
                        title: $"{titleBase} — You are winning",
                        message: msg,
                        auctionId: invAuc.AuctionId,
                        inventoryAuctionId: invAuc.InventoryAuctionId,
                        createdByUserId: currentBidderId);
                }
            }
        }

        #endregion

        #region Endpoints

        /// <summary>
        /// Places a new bid.
        /// Also triggers bid-winning / bid-outbid notifications via SignalR.
        /// </summary>
        [HttpPost("add")]
        public async Task<int> Add([FromBody] AuctionBid bid)
        {
            var userId = GetCurrentUserId();
            if (userId <= 0)
            {
                // shouldn't happen thanks to [Authorize], but guard anyway
                return 0;
            }

            // trust JWT, not client
            bid.CreatedById = userId;

            // Find previous winning bid for this lot BEFORE inserting the new one
            AuctionBid? previousWinning = null;
            if (bid.InventoryAuctionId > 0)
            {
                var allBids = await _auctionBidApplication.GetList(new AuctionBid());

                previousWinning = allBids
                    .Where(b => b.InventoryAuctionId == bid.InventoryAuctionId && b.Active)
                    .OrderByDescending(b => b.CreatedDate ?? DateTime.MinValue)
                    .ThenByDescending(b => b.AuctionBidId)
                    .FirstOrDefault(b =>
                        (b.AuctionBidStatusName ?? string.Empty)
                            .Equals("Winning", StringComparison.OrdinalIgnoreCase));
            }

            // DB/SP will:
            //  - insert the new bid
            //  - update statuses (old winning -> Outbid, etc.)
            var newId = await _auctionBidApplication.Add(bid);

            // At this point `bid` should be populated by infra Add()
            // with AuctionBidStatusId, AuctionBidStatusName, etc.
            await SendBidNotificationsAsync(bid, previousWinning);

            return newId;
        }

        [HttpPut("update")]
        public async Task<bool> Update([FromBody] AuctionBid auctionBid)
        {
            var response = await _auctionBidApplication.Update(auctionBid);
            return response;
        }

        [HttpPut("activate")]
        public async Task<bool> Activate([FromBody] AuctionBid auctionBid)
        {
            var response = await _auctionBidApplication.Activate(auctionBid);
            return response;
        }

        [HttpGet("get")]
        public async Task<AuctionBid> Get([FromQuery] AuctionBid request)
        {
            AuctionBid response = await _auctionBidApplication.Get(request);
            return response;
        }

        [HttpGet("getlist")]
        public async Task<List<AuctionBid>> GetList([FromQuery] AuctionBid request)
        {
            List<AuctionBid> response = await _auctionBidApplication.GetList(request);
            return response;
        }

        #endregion
    }
}
