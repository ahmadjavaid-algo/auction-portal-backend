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
   
    public class NotificationsController : APIBaseController
    {
        #region Constructor

 
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


        private int GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimsConstants.UserIdClaimType)
                        ?? User.FindFirst(ClaimTypes.NameIdentifier);

            return claim != null && int.TryParse(claim.Value, out var id) ? id : 0;
        }

        #endregion

        #region Endpoints

        [HttpGet("list")]
        public async Task<List<Notification>> GetForCurrentUser(
            [FromQuery] bool unreadOnly = false,
            [FromQuery] int top = 50)
        {
            var userId = GetCurrentUserId();
            if (userId <= 0)
            {
             
                return new List<Notification>();
            }

            var notifications = await NotificationApplication.GetForUser(userId, unreadOnly, top);
            return notifications;
        }


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
