// AuctionPortal.Controllers/AdminNotificationsController.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using AuctionPortal.ApplicationLayer.IApplication;
using AuctionPortal.Common.Controllers;
using AuctionPortal.Common.Core;
using AuctionPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuctionPortal.Controllers
{
    [Authorize] // app-user login (admin portal)
    public class AdminNotificationsController : APIBaseController
    {
        public IAdminNotificationApplication AdminNotificationApplication { get; }

        public AdminNotificationsController(
            IAdminNotificationApplication adminNotificationApplication,
            IHeaderValue headerValue,
            IConfiguration configuration)
            : base(headerValue, configuration)
        {
            AdminNotificationApplication = adminNotificationApplication;
        }

        /// <summary>
        /// Returns admin notifications (global; optionally unread-only).
        /// GET /api/AdminNotifications/list?unreadOnly=true&top=50
        /// </summary>
        [HttpGet("list")]
        public async Task<List<AdminNotification>> GetList(
            [FromQuery] bool unreadOnly = false,
            [FromQuery] int top = 50)
        {
            return await AdminNotificationApplication.GetList(unreadOnly, top);
        }

        /// <summary>
        /// Marks all admin notifications as read and returns the updated list.
        /// POST /api/AdminNotifications/mark-all-read
        /// </summary>
        [HttpPost("mark-all-read")]
        public async Task<List<AdminNotification>> MarkAllRead()
        {
            var updated = await AdminNotificationApplication.MarkAllRead(null);
            return updated;
        }

        /// <summary>
        /// Clears (soft-deletes) all admin notifications and returns remaining (usually empty).
        /// POST /api/AdminNotifications/clear-all
        /// </summary>
        [HttpPost("clear-all")]
        public async Task<List<AdminNotification>> ClearAll()
        {
            var updated = await AdminNotificationApplication.ClearAll(null);
            return updated;
        }
    }
}
