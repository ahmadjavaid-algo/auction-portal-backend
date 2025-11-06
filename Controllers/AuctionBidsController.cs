using AuctionPortal.ApplicationLayer.IApplication;
using AuctionPortal.Common.Controllers;
using AuctionPortal.Common.Core;
using AuctionPortal.Common.Models;
using AuctionPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuctionPortal.Controllers
{
    public class AuctionBidsController : APIBaseController
    {
        #region Constructor
        /// <summary>
        /// AuctionBidsController initializes class object.
        /// </summary>
        public AuctionBidsController(IAuctionBidApplication AuctionBidApplication, IHeaderValue headerValue, IConfiguration configuration)
            : base(headerValue, configuration)
        {
            this.AuctionBidApplication = AuctionBidApplication;
        }
        #endregion

        #region Properties and Data Members
        public IAuctionBidApplication AuctionBidApplication { get; }
        #endregion

        [HttpPost("add")]
        public async Task<int> Add([FromBody] AuctionBid AuctionBid)
        {
            var AuctionBidId = await this.AuctionBidApplication.Add(AuctionBid);
            return AuctionBidId;
        }

        [HttpPut("update")]
        public async Task<bool> Update([FromBody] AuctionBid AuctionBid)
        {
            var response = await AuctionBidApplication.Update(AuctionBid);
            return response;
        }

        [HttpPut("activate")]
        public async Task<bool> Activate([FromBody] AuctionBid AuctionBid)
        {
            var response = await AuctionBidApplication.Activate(AuctionBid);
            return response;
        }

        [HttpGet("get")]
        public async Task<AuctionBid> Get([FromQuery] AuctionBid request)
        {
            AuctionBid response = await this.AuctionBidApplication.Get(request);
            return response;
        }

        [HttpGet("getlist")]
        public async Task<List<AuctionBid>> GetList([FromQuery] AuctionBid request)
        {
            List<AuctionBid> response = await this.AuctionBidApplication.GetList(request);
            return response;
        }
    }
}
