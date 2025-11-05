using System.Security.Claims;
using AuctionPortal.ApplicationLayer.IApplication;
using AuctionPortal.Common.Auth;
using AuctionPortal.Common.Controllers;
using AuctionPortal.Common.Core;
using AuctionPortal.Common.Models;
using AuctionPortal.Hubs;
using AuctionPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace AuctionPortal.Controllers
{
    [Authorize] // JWT required
    public class FavouritesController : APIBaseController
    {
        #region Constructor

        public FavouritesController(
            IFavouriteApplication favouriteApplication,
            IInventoryAuctionApplication inventoryAuctionApplication,
            IAuctionApplication auctionApplication,
            INotificationApplication notificationApplication,
            IHubContext<NotificationHub> hub,
            IHeaderValue headerValue,
            IConfiguration configuration)
            : base(headerValue, configuration)
        {
            FavouriteApplication = favouriteApplication;
            _inventoryAuctionApplication = inventoryAuctionApplication;
            _auctionApplication = auctionApplication;
            _notificationApplication = notificationApplication;
            _hub = hub;
        }

        #endregion

        #region Properties and Data Members

        public IFavouriteApplication FavouriteApplication { get; }
        private readonly IInventoryAuctionApplication _inventoryAuctionApplication;
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
        /// Persist a Notification row in the DB.
        /// </summary>
        private async Task<int> AddNotificationRowAsync(
            int userId,
            string type,
            string title,
            string message,
            int? auctionId = null,
            int? inventoryAuctionId = null)
        {
            var notification = new Notification
            {
                UserId = userId,
                Type = type,
                Title = title,
                Message = message,
                IsRead = false,
                CreatedById = userId,
                AuctionId = auctionId,
                InventoryAuctionId = inventoryAuctionId
            };

            var id = await _notificationApplication.Add(notification);
            return id;
        }


        /// <summary>
        /// Builds a FavouriteNotification + message and:
        ///  1) stores Notification row in DB
        ///  2) pushes "FavouriteAdded" via SignalR
        /// Used for both first-time add and re-activation.
        /// </summary>
        private async Task SendFavouriteNotificationAsync(
    int userId,
    int favouriteId,
    int inventoryAuctionId)
        {
            var invAuc = await _inventoryAuctionApplication.Get(new InventoryAuction
            {
                InventoryAuctionId = inventoryAuctionId
            });

            if (invAuc == null)
                return;

            var timebox = await _auctionApplication.GetTimebox(new Auction
            {
                AuctionId = invAuc.AuctionId
            });

            var titleText = $"{timebox?.AuctionName ?? "Auction"} — Lot #{invAuc.InventoryId}";

            var notification = new FavouriteNotification
            {
                FavouriteId = favouriteId,
                UserId = userId,
                InventoryAuctionId = invAuc.InventoryAuctionId,
                AuctionId = invAuc.AuctionId,
                Title = titleText,
                StartEpochMsUtc = timebox?.StartEpochMsUtc,
                EndEpochMsUtc = timebox?.EndEpochMsUtc
            };

            var lotLabel = titleText;
            var msg = $"{lotLabel} has been added to your favourites.";

            if (timebox != null)
            {
                if (timebox.StartEpochMsUtc > 0 && timebox.EndEpochMsUtc > 0)
                {
                    var start = DateTimeOffset.FromUnixTimeMilliseconds(timebox.StartEpochMsUtc).UtcDateTime;
                    var end = DateTimeOffset.FromUnixTimeMilliseconds(timebox.EndEpochMsUtc).UtcDateTime;
                    msg += $" Auction runs {start:u} → {end:u}.";
                }
                else if (timebox.StartEpochMsUtc > 0)
                {
                    var start = DateTimeOffset.FromUnixTimeMilliseconds(timebox.StartEpochMsUtc).UtcDateTime;
                    msg += $" Auction starts at {start:u}.";
                }
            }

            // tie to auction/lot
            await AddNotificationRowAsync(
                userId,
                type: "favourite-added",
                title: titleText,
                message: msg,
                auctionId: invAuc.AuctionId,
                inventoryAuctionId: invAuc.InventoryAuctionId
            );

            await _hub.Clients
                .User(userId.ToString())
                .SendAsync("FavouriteAdded", notification);
        }


        /// <summary>
        /// Creates & stores a "favourite-deactivated" Notification row and
        /// pushes a "FavouriteDeactivated" event via SignalR.
        /// </summary>
        private async Task SendFavouriteRemovedNotificationAsync(
     int userId,
     Favourite favourite)
        {
            var invAuc = await _inventoryAuctionApplication.Get(new InventoryAuction
            {
                InventoryAuctionId = favourite.InventoryAuctionId
            });

            string titleText;
            string messageText;
            int? auctionId = null;

            if (invAuc != null)
            {
                var timebox = await _auctionApplication.GetTimebox(new Auction
                {
                    AuctionId = invAuc.AuctionId
                });

                var auctionName = timebox?.AuctionName ?? "Auction";
                titleText = $"{auctionName} — Lot #{invAuc.InventoryId}";
                messageText = $"You removed {titleText} from your favourites.";
                auctionId = invAuc.AuctionId;
            }
            else
            {
                titleText = "Favourite removed";
                messageText = $"You removed lot #{favourite.InventoryAuctionId} from your favourites.";
            }

            await AddNotificationRowAsync(
                userId,
                type: "favourite-deactivated",
                title: titleText,
                message: messageText,
                auctionId: auctionId,
                inventoryAuctionId: favourite.InventoryAuctionId
            );

            await _hub.Clients
                .User(userId.ToString())
                .SendAsync("FavouriteDeactivated", new
                {
                    favouriteId = favourite.BidderInventoryAuctionFavoriteId,
                    userId,
                    inventoryAuctionId = favourite.InventoryAuctionId
                });
        }


        #endregion

        #region Endpoints

        [HttpPost("add")]
        public async Task<int> Add([FromBody] Favourite favourite)
        {
            var userId = GetCurrentUserId();
            favourite.UserId = userId; // trust JWT, not client

            var favouriteId = await this.FavouriteApplication.Add(favourite);

            // persist + push real-time notification (first-time add)
            await SendFavouriteNotificationAsync(
                userId,
                favouriteId,
                favourite.InventoryAuctionId);

            return favouriteId;
        }

        [HttpPut("update")]
        public async Task<bool> Update([FromBody] Favourite favourite)
        {
            var response = await FavouriteApplication.Update(favourite);
            return response;
        }

        [HttpPut("activate")]
        public async Task<bool> Activate([FromBody] Favourite favourite)
        {
            var response = await FavouriteApplication.Activate(favourite);
            if (!response)
            {
                return false;
            }

            var userId = GetCurrentUserId();

            // Load the full favourite row so we have InventoryAuctionId etc.
            var dbFavourite = await FavouriteApplication.Get(new Favourite
            {
                BidderInventoryAuctionFavoriteId = favourite.BidderInventoryAuctionFavoriteId
            });

            if (dbFavourite == null)
            {
                // State already toggled, nothing more we can do
                return true;
            }

            if (favourite.Active)
            {
                // RE-ACTIVATED: treat like "added" again
                await SendFavouriteNotificationAsync(
                    userId,
                    dbFavourite.BidderInventoryAuctionFavoriteId,
                    dbFavourite.InventoryAuctionId);
            }
            else
            {
                // DEACTIVATED: store + broadcast removal notification
                await SendFavouriteRemovedNotificationAsync(userId, dbFavourite);
            }

            return true;
        }

        [HttpGet("get")]
        public async Task<Favourite> Get([FromQuery] Favourite request)
        {
            Favourite response = await this.FavouriteApplication.Get(request);
            return response;
        }

        [HttpGet("getlist")]
        public async Task<List<Favourite>> GetList([FromQuery] Favourite request)
        {
            List<Favourite> response = await this.FavouriteApplication.GetList(request);
            return response;
        }

        #endregion
    }
}
