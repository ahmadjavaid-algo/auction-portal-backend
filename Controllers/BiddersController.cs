using AuctionPortal.ApplicationLayer.Application;
using AuctionPortal.ApplicationLayer.IApplication;
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
    public class BiddersController : APIBaseController
    {
        #region Constructor
        /// <summary>
        /// BiddersController initializes class object.
        /// </summary>
        public BiddersController(
            IBidderApplication bidderApplication,
            IHeaderValue headerValue,
            IConfiguration configuration,
            IAdminNotificationApplication adminNotificationApplication,
            IHubContext<NotificationHub> hubContext)
            : base(headerValue, configuration)
        {
            BidderApplication = bidderApplication;
            _adminNotificationApplication = adminNotificationApplication;
            _hubContext = hubContext;
        }
        #endregion

        #region Properties and Data Members
        public IBidderApplication BidderApplication { get; }

        private readonly IAdminNotificationApplication _adminNotificationApplication;
        private readonly IHubContext<NotificationHub> _hubContext;
        #endregion

        [HttpPost("add")]
        public async Task<int> Add([FromBody] Bidder bidder)
        {
            var bidderId = await BidderApplication.Add(bidder);

            // Admin notification: new bidder registered
            if (bidderId > 0)
            {
                var title = "New bidder registered";

                var fullName = $"{bidder.FirstName} {bidder.LastName}".Trim();
                var email = bidder.Email ?? string.Empty;

                var message =
                    $"Bidder #{bidderId} ({fullName}, {email}) has just signed up.";

                await AdminNotificationHelper.CreateAndBroadcastAsync(
                    _adminNotificationApplication,
                    _hubContext,
                    type: "bidder-created",
                    title: title,
                    message: message,
                    affectedUserId: bidderId,
                    auctionId: null,
                    inventoryAuctionId: null);
            }

            return bidderId;
        }

        [HttpPut("update")]
        public async Task<bool> Update([FromBody] Bidder bidder)
        {
            var response = await BidderApplication.Update(bidder);
            return response;
        }

        [HttpPut("activate")]
        public async Task<bool> Activate([FromBody] Bidder bidder)
        {
            var response = await BidderApplication.Activate(bidder);
            return response;
        }

        [HttpGet("get")]
        public async Task<Bidder> Get([FromQuery] Bidder request)
        {
            var response = await BidderApplication.Get(request);
            return response;
        }

        [HttpGet("getlist")]
        public async Task<List<Bidder>> GetList([FromQuery] Bidder request)
        {
            var response = await BidderApplication.GetList(request);
            return response;
        }

        [HttpGet("getstats")]
        public async Task<Bidder> GetStats()
        {
            return await BidderApplication.GetStats();
        }
    }
}
