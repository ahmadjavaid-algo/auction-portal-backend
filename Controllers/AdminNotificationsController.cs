
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


        [HttpGet("list")]
        public async Task<List<AdminNotification>> GetList(
            [FromQuery] bool unreadOnly = false,
            [FromQuery] int top = 50)
        {
            return await AdminNotificationApplication.GetList(unreadOnly, top);
        }


        [HttpPost("mark-all-read")]
        public async Task<List<AdminNotification>> MarkAllRead()
        {
            var updated = await AdminNotificationApplication.MarkAllRead(null);
            return updated;
        }


        [HttpPost("clear-all")]
        public async Task<List<AdminNotification>> ClearAll()
        {
            var updated = await AdminNotificationApplication.ClearAll(null);
            return updated;
        }
        [HttpGet("history")]
        public Task<List<AdminNotification>> History([FromQuery] int top = 200) =>
            AdminNotificationApplication.GetHistory(top);
    }
}
