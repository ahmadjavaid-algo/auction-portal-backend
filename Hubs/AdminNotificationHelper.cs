// AuctionPortal.Hubs/AdminNotificationHelper.cs
using System.Threading.Tasks;
using AuctionPortal.ApplicationLayer.IApplication;
using AuctionPortal.Models;
using Microsoft.AspNetCore.SignalR;

namespace AuctionPortal.Hubs
{
    public static class AdminNotificationHelper
    {
        public static async Task CreateAndBroadcastAsync(
            IAdminNotificationApplication adminNotifApp,
            IHubContext<NotificationHub> hub,
            string type,
            string title,
            string message,
            int? affectedUserId,
            int? auctionId,
            int? inventoryAuctionId)
        {
            var adminNotification = new AdminNotification
            {
                Type = type,
                Title = title,
                Message = message,
                IsRead = false,
                CreatedById = affectedUserId,
                AffectedUserId = affectedUserId,
                AuctionId = auctionId,
                InventoryAuctionId = inventoryAuctionId
            };

            await adminNotifApp.Add(adminNotification);

            await hub.Clients
                .Group("admins")
                .SendAsync("AdminNotificationCreated", adminNotification);
        }
    }
}
