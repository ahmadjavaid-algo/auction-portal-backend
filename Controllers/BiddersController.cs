using AuctionPortal.ApplicationLayer.Application;
using AuctionPortal.ApplicationLayer.IApplication;
using AuctionPortal.Common.Controllers;
using AuctionPortal.Common.Core;
using AuctionPortal.Common.Models;
using AuctionPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuctionPortal.Controllers
{
    public class BiddersController : APIBaseController
    {
        #region Constructor
        /// <summary>
        /// BiddersController initializes class object.
        /// </summary>
        public BiddersController(IBidderApplication BidderApplication, IHeaderValue headerValue, IConfiguration configuration)
            : base(headerValue, configuration)
        {
            this.BidderApplication = BidderApplication;
        }
        #endregion

        #region Properties and Data Members
        public IBidderApplication BidderApplication { get; }
        #endregion

        [HttpPost("add")]
        public async Task<int> Add([FromBody] Bidder Bidder)
        {
            var applicationRoleId = await this.BidderApplication.Add(Bidder);
            return applicationRoleId;
        }

        [HttpPut("update")]
        public async Task<bool> Update([FromBody] Bidder Bidder)
        {
            var response = await this.BidderApplication.Update(Bidder);
            return response;
        }

        [HttpPut("activate")]
        public async Task<bool> Activate([FromBody] Bidder Bidder)
        {
            var response = await this.BidderApplication.Activate(Bidder);
            return response;
        }

        [HttpGet("get")]
        public async Task<Bidder> Get([FromQuery] Bidder request)
        {
            var response = await this.BidderApplication.Get(request);
            return response;
        }

        [HttpGet("getlist")]
        public async Task<List<Bidder>> GetList([FromQuery] Bidder request)
        {
            var response = await this.BidderApplication.GetList(request);
            return response;
        }
        [HttpGet("getstats")]
        public async Task<Bidder> GetStats()
        {
            return await this.BidderApplication.GetStats();
        }
    }
}
