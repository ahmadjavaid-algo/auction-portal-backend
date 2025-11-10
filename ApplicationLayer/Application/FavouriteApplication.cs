using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AuctionPortal.ApplicationLayer.IApplication;
using AuctionPortal.Hubs;
using AuctionPortal.InfrastructureLayer.Interfaces;
using AuctionPortal.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;

namespace AuctionPortal.ApplicationLayer.Application
{
    public class FavouriteApplication : BaseApplication, IFavouriteApplication
    {
        #region Constructor

        public FavouriteApplication(
            IFavouriteInfrastructure favouriteInfrastructure,
            IInventoryAuctionApplication inventoryAuctionApplication,
            IAuctionApplication auctionApplication,
            INotificationApplication notificationApplication,
            IHubContext<NotificationHub> hub,
            IConfiguration configuration)
            : base(configuration)
        {
            if (favouriteInfrastructure == null)
            {
                throw new ArgumentNullException(nameof(favouriteInfrastructure));
            }
            if (inventoryAuctionApplication == null)
            {
                throw new ArgumentNullException(nameof(inventoryAuctionApplication));
            }
            if (auctionApplication == null)
            {
                throw new ArgumentNullException(nameof(auctionApplication));
            }
            if (notificationApplication == null)
            {
                throw new ArgumentNullException(nameof(notificationApplication));
            }
            if (hub == null)
            {
                throw new ArgumentNullException(nameof(hub));
            }

            this.FavouriteInfrastructure = favouriteInfrastructure;
            this.InventoryAuctionApplication = inventoryAuctionApplication;
            this.AuctionApplication = auctionApplication;
            this.NotificationApplication = notificationApplication;
            this.Hub = hub;
        }

        #endregion

        #region Properties and Data Members

        public IFavouriteInfrastructure FavouriteInfrastructure { get; }
        public IInventoryAuctionApplication InventoryAuctionApplication { get; }
        public IAuctionApplication AuctionApplication { get; }
        public INotificationApplication NotificationApplication { get; }
        public IHubContext<NotificationHub> Hub { get; }

        #endregion

        #region Queries

        public async Task<Favourite> Get(Favourite entity)
        {
            return await FavouriteInfrastructure.Get(entity);
        }

        public async Task<List<Favourite>> GetList(Favourite entity)
        {
            return await FavouriteInfrastructure.GetList(entity);
        }

        #endregion

        #region Commands

        public async Task<int> Add(Favourite entity)
        {
            return await FavouriteInfrastructure.Add(entity);
        }

        public async Task<bool> Update(Favourite entity)
        {
            return await FavouriteInfrastructure.Update(entity);
        }

        public async Task<bool> Activate(Favourite entity)
        {
            return await FavouriteInfrastructure.Activate(entity);
        }

        #endregion

        #region Use-case Methods (with notifications and SignalR)

        /// <summary>
        /// Adds a favourite for a specific user and sends "favourite-added" notification
        /// plus real-time SignalR event.
        /// </summary>
        public async Task<int> AddForUser(Favourite entity, int userId)
        {
            entity.UserId = userId;
            entity.CreatedById = userId;

            var favouriteId = await FavouriteInfrastructure.Add(entity);

            await SendFavouriteAddedNotification(userId, favouriteId, entity.InventoryAuctionId);

            return favouriteId;
        }

        /// <summary>
        /// Toggles Active for a favourite and sends appropriate notifications
        /// (favourite-added on reactivation, favourite-deactivated on removal).
        /// </summary>
        public async Task<bool> ToggleActiveForUser(Favourite entity, int userId)
        {
            var success = await FavouriteInfrastructure.Activate(entity);
            if (!success)
            {
                return false;
            }

            // Reload so we have full data such as InventoryAuctionId
            var dbFavourite = await FavouriteInfrastructure.Get(new Favourite
            {
                BidderInventoryAuctionFavoriteId = entity.BidderInventoryAuctionFavoriteId
            });

            if (dbFavourite == null)
            {
                // State has been toggled but row is not found, nothing more to do
                return true;
            }

            if (entity.Active)
            {
                // Re-activated: treat as added again
                await SendFavouriteAddedNotification(
                    userId,
                    dbFavourite.BidderInventoryAuctionFavoriteId,
                    dbFavourite.InventoryAuctionId);
            }
            else
            {
                // Deactivated: send removal notification
                await SendFavouriteRemovedNotification(userId, dbFavourite);
            }

            return true;
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// Persists a Notification row in the DB.
        /// </summary>
        private async Task<int> AddNotificationRow(
            int userId,
            string type,
            string title,
            string message,
            int? auctionId,
            int? inventoryAuctionId)
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

            var id = await NotificationApplication.Add(notification);
            return id;
        }

        /// <summary>
        /// Builds a FavouriteNotification, stores a "favourite-added" Notification row,
        /// and pushes a "FavouriteAdded" SignalR event.
        /// Used for both first-time add and re-activation.
        /// </summary>
        private async Task SendFavouriteAddedNotification(
            int userId,
            int favouriteId,
            int inventoryAuctionId)
        {
            var invAuc = await InventoryAuctionApplication.Get(new InventoryAuction
            {
                InventoryAuctionId = inventoryAuctionId
            });

            if (invAuc == null)
            {
                return;
            }

            var timebox = await AuctionApplication.GetTimebox(new Auction
            {
                AuctionId = invAuc.AuctionId
            });

            string auctionName = "Auction";
            if (timebox != null && !string.IsNullOrEmpty(timebox.AuctionName))
            {
                auctionName = timebox.AuctionName;
            }

            string titleText = string.Format("{0} — Lot #{1}", auctionName, invAuc.InventoryId);

            var favouriteNotification = new FavouriteNotification
            {
                FavouriteId = favouriteId,
                UserId = userId,
                InventoryAuctionId = invAuc.InventoryAuctionId,
                AuctionId = invAuc.AuctionId,
                Title = titleText,
                StartEpochMsUtc = timebox != null ? timebox.StartEpochMsUtc : (long?)null,
                EndEpochMsUtc = timebox != null ? timebox.EndEpochMsUtc : (long?)null
            };

            string msg = string.Format("{0} has been added to your favourites.", titleText);

            if (timebox != null)
            {
                if (timebox.StartEpochMsUtc > 0 && timebox.EndEpochMsUtc > 0)
                {
                    var start = DateTimeOffset.FromUnixTimeMilliseconds(timebox.StartEpochMsUtc).UtcDateTime;
                    var end = DateTimeOffset.FromUnixTimeMilliseconds(timebox.EndEpochMsUtc).UtcDateTime;
                    msg += string.Format(" Auction runs {0:u} → {1:u}.", start, end);
                }
                else if (timebox.StartEpochMsUtc > 0)
                {
                    var start = DateTimeOffset.FromUnixTimeMilliseconds(timebox.StartEpochMsUtc).UtcDateTime;
                    msg += string.Format(" Auction starts at {0:u}.", start);
                }
            }

            await AddNotificationRow(
                userId,
                "favourite-added",
                titleText,
                msg,
                invAuc.AuctionId,
                invAuc.InventoryAuctionId);

            await Hub.Clients
                .User(userId.ToString())
                .SendAsync("FavouriteAdded", favouriteNotification);
        }

        /// <summary>
        /// Creates and stores a "favourite-deactivated" Notification row and pushes
        /// a "FavouriteDeactivated" SignalR event.
        /// </summary>
        private async Task SendFavouriteRemovedNotification(
            int userId,
            Favourite favourite)
        {
            var invAuc = await InventoryAuctionApplication.Get(new InventoryAuction
            {
                InventoryAuctionId = favourite.InventoryAuctionId
            });

            string titleText;
            string messageText;
            int? auctionId = null;

            if (invAuc != null)
            {
                var timebox = await AuctionApplication.GetTimebox(new Auction
                {
                    AuctionId = invAuc.AuctionId
                });

                string auctionName = "Auction";
                if (timebox != null && !string.IsNullOrEmpty(timebox.AuctionName))
                {
                    auctionName = timebox.AuctionName;
                }

                titleText = string.Format("{0} — Lot #{1}", auctionName, invAuc.InventoryId);
                messageText = string.Format("You removed {0} from your favourites.", titleText);
                auctionId = invAuc.AuctionId;
            }
            else
            {
                titleText = "Favourite removed";
                messageText = string.Format(
                    "You removed lot #{0} from your favourites.",
                    favourite.InventoryAuctionId);
            }

            await AddNotificationRow(
                userId,
                "favourite-deactivated",
                titleText,
                messageText,
                auctionId,
                favourite.InventoryAuctionId);

            await Hub.Clients
                .User(userId.ToString())
                .SendAsync("FavouriteDeactivated", new
                {
                    favouriteId = favourite.BidderInventoryAuctionFavoriteId,
                    userId = userId,
                    inventoryAuctionId = favourite.InventoryAuctionId
                });
        }

        #endregion
    }
}
