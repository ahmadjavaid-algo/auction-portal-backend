// AuctionPortal.Controllers/NotificationsController.cs
using System.Security.Claims;
using AuctionPortal.ApplicationLayer.IApplication;
using AuctionPortal.Common.Auth;
using AuctionPortal.Common.Controllers;
using AuctionPortal.Common.Core;
using AuctionPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuctionPortal.Controllers
{
    [Authorize] // require JWT (same as FavouritesController)
    public class NotificationsController : APIBaseController
    {
        #region Constructor

        /// <summary>
        /// NotificationsController initializes class object.
        /// </summary>
        public NotificationsController(
            INotificationApplication notificationApplication,
            IHeaderValue headerValue,
            IConfiguration configuration)
            : base(headerValue, configuration)
        {
            NotificationApplication = notificationApplication;
        }

        #endregion

        #region Properties and Data Members

        public INotificationApplication NotificationApplication { get; }

        #endregion

        #region Helpers

        /// <summary>
        /// Gets current userId from JWT claims.
        /// </summary>
        private int GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimsConstants.UserIdClaimType)
                        ?? User.FindFirst(ClaimTypes.NameIdentifier);

            return claim != null && int.TryParse(claim.Value, out var id) ? id : 0;
        }

        #endregion

        #region Endpoints

        /// <summary>
        /// Returns notifications for the current user (optionally unread-only).
        /// GET /api/Notifications/list?unreadOnly=true&top=50
        /// </summary>
        [HttpGet("list")]
        public async Task<List<Notification>> GetForCurrentUser(
            [FromQuery] bool unreadOnly = false,
            [FromQuery] int top = 50)
        {
            var userId = GetCurrentUserId();
            if (userId <= 0)
            {
                // Should not normally happen because of [Authorize],
                // but we guard anyway.
                return new List<Notification>();
            }

            var notifications = await NotificationApplication.GetForUser(userId, unreadOnly, top);
            return notifications;
        }

        /// <summary>
        /// Marks all notifications for the current user as read and returns the updated list.
        /// POST /api/Notifications/mark-all-read
        /// </summary>
        [HttpPost("mark-all-read")]
        public async Task<List<Notification>> MarkAllRead()
        {
            var userId = GetCurrentUserId();
            if (userId <= 0)
            {
                return new List<Notification>();
            }

            var updated = await NotificationApplication.MarkAllRead(userId, userId);
            return updated;
        }

        /// <summary>
        /// Clears (soft-deletes) all notifications for the current user and returns the updated list (usually empty).
        /// POST /api/Notifications/clear-all
        /// </summary>
        [HttpPost("clear-all")]
        public async Task<List<Notification>> ClearAll()
        {
            var userId = GetCurrentUserId();
            if (userId <= 0)
            {
                return new List<Notification>();
            }

            var updated = await NotificationApplication.ClearAll(userId, userId);
            return updated;
        }

        #endregion
    }
}
