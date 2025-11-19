using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuctionPortal.ApplicationLayer.IApplication;
using AuctionPortal.Hubs;
using AuctionPortal.InfrastructureLayer.Interfaces;
using AuctionPortal.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;

namespace AuctionPortal.ApplicationLayer.Application
{
    public class AuctionBidApplication : BaseApplication, IAuctionBidApplication
    {
        public AuctionBidApplication(
            IAuctionBidInfrastructure AuctionBidInfrastructure,
            IInventoryAuctionApplication InventoryAuctionApplication,
            IAuctionApplication AuctionApplication,
            INotificationApplication NotificationApplication,
            IHubContext<NotificationHub> hub,
            IConfiguration configuration)
            : base(configuration)
        {
            this.AuctionBidInfrastructure = AuctionBidInfrastructure ?? throw new ArgumentNullException(nameof(AuctionBidInfrastructure));
            this.InventoryAuctionApplication = InventoryAuctionApplication ?? throw new ArgumentNullException(nameof(InventoryAuctionApplication));
            this.AuctionApplication = AuctionApplication ?? throw new ArgumentNullException(nameof(AuctionApplication));
            this.NotificationApplication = NotificationApplication ?? throw new ArgumentNullException(nameof(NotificationApplication));
            this.Hub = hub ?? throw new ArgumentNullException(nameof(hub));
        }

        public IAuctionBidInfrastructure AuctionBidInfrastructure { get; }
        public IInventoryAuctionApplication InventoryAuctionApplication { get; }
        public IAuctionApplication AuctionApplication { get; }
        public INotificationApplication NotificationApplication { get; }
        public IHubContext<NotificationHub> Hub { get; }

        #region Queries

        public async Task<AuctionBid> Get(AuctionBid entity)
        {
            return await AuctionBidInfrastructure.Get(entity);
        }

        public async Task<List<AuctionBid>> GetList(AuctionBid entity)
        {
            return await AuctionBidInfrastructure.GetList(entity);
        }

        #endregion

        #region Commands

        /// <summary>
        /// Places a bid and performs all related operations
        /// </summary>
        public async Task<int> Add(AuctionBid entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            AuctionBid previousWinning = null;

            if (entity.InventoryAuctionId > 0)
            {
                var allBids = await AuctionBidInfrastructure.GetList(new AuctionBid());

                previousWinning = allBids
                    .Where(b => b.InventoryAuctionId == entity.InventoryAuctionId && b.Active)
                    .OrderByDescending(b => b.CreatedDate ?? DateTime.MinValue)
                    .ThenByDescending(b => b.AuctionBidId)
                    .FirstOrDefault(b =>
                        (b.AuctionBidStatusName ?? string.Empty)
                            .Equals("Winning", StringComparison.OrdinalIgnoreCase));
            }

            var newId = await AuctionBidInfrastructure.Add(entity);

            await SendBidNotificationsAsync(entity, previousWinning);

            return newId;
        }

        public async Task<bool> Update(AuctionBid entity)
        {
            return await AuctionBidInfrastructure.Update(entity);
        }

        public async Task<bool> Activate(AuctionBid entity)
        {
            return await AuctionBidInfrastructure.Activate(entity);
        }

        #endregion

        #region Helpers (application-level orchestration)

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

            await NotificationApplication.Add(notification);

            await Hub.Clients
                .User(targetUserId.ToString())
                .SendAsync("NotificationCreated", notification);
        }

        private async Task SendBidNotificationsAsync(AuctionBid newBid, AuctionBid previousWinningBid)
        {
            if (newBid == null || newBid.InventoryAuctionId <= 0)
            {
                return;
            }

            var invAuc = await InventoryAuctionApplication.Get(new InventoryAuction
            {
                InventoryAuctionId = newBid.InventoryAuctionId
            });

            if (invAuc == null)
            {
                return;
            }

            var timebox = await AuctionApplication.GetTimebox(new Auction
            {
                AuctionId = invAuc.AuctionId
            });

            var auctionName = timebox != null && !string.IsNullOrWhiteSpace(timebox.AuctionName)
                ? timebox.AuctionName
                : "Auction";

            var titleBase = string.Format("{0} — Lot #{1}", auctionName, invAuc.InventoryId);

            var status = newBid.AuctionBidStatusName;
            if (status == null)
            {
                status = string.Empty;
            }

            status = status.Trim();

            
            if (previousWinningBid != null &&
                previousWinningBid.CreatedById.HasValue &&
                previousWinningBid.CreatedById.Value != (newBid.CreatedById ?? 0) &&
                status.Equals("Winning", StringComparison.OrdinalIgnoreCase))
            {
                var oldWinnerId = previousWinningBid.CreatedById.Value;

                var existing = await NotificationApplication.GetForUser(oldWinnerId, false, 200);
                bool alreadyHasOutbid = existing.Any(n =>
                    string.Equals(n.Type, "bid-outbid", StringComparison.OrdinalIgnoreCase) &&
                    n.AuctionId == invAuc.AuctionId &&
                    n.InventoryAuctionId == invAuc.InventoryAuctionId);

                if (!alreadyHasOutbid)
                {
                    var msg = string.Format(
                        "You have been outbid on {0}. Your previous bid of {1:N2} is no longer winning.",
                        titleBase,
                        previousWinningBid.BidAmount);

                    await CreateAndPushNotificationAsync(
                        oldWinnerId,
                        "bid-outbid",
                        string.Format("{0} — You were outbid", titleBase),
                        msg,
                        invAuc.AuctionId,
                        invAuc.InventoryAuctionId,
                        newBid.CreatedById ?? oldWinnerId);
                }
            }

            
            if (status.Equals("Winning", StringComparison.OrdinalIgnoreCase) &&
                newBid.CreatedById.HasValue)
            {
                var currentBidderId = newBid.CreatedById.Value;

                var existing = await NotificationApplication.GetForUser(currentBidderId, false, 200);
                bool alreadyHasWinning = existing.Any(n =>
                    string.Equals(n.Type, "bid-winning", StringComparison.OrdinalIgnoreCase) &&
                    n.AuctionId == invAuc.AuctionId &&
                    n.InventoryAuctionId == invAuc.InventoryAuctionId);

                if (!alreadyHasWinning)
                {
                    var msg = string.Format(
                        "Your bid of {0:N2} is currently winning for {1}.",
                        newBid.BidAmount,
                        titleBase);

                    await CreateAndPushNotificationAsync(
                        currentBidderId,
                        "bid-winning",
                        string.Format("{0} — You are winning", titleBase),
                        msg,
                        invAuc.AuctionId,
                        invAuc.InventoryAuctionId,
                        currentBidderId);
                }
            }
        }

        #endregion
    }
}
