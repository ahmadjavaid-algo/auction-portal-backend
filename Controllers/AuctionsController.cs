using AuctionPortal.ApplicationLayer.IApplication;
using AuctionPortal.Common.Controllers;
using AuctionPortal.Common.Core;
using AuctionPortal.Common.Models;
using AuctionPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuctionPortal.Controllers
{
    public class AuctionsController : APIBaseController
    {
        #region Constructor

        public AuctionsController(IAuctionApplication AuctionApplication, IHeaderValue headerValue, IConfiguration configuration)
            : base(headerValue, configuration)
        {
            this.AuctionApplication = AuctionApplication;
        }
        #endregion

        #region Properties and Data Members
        public IAuctionApplication AuctionApplication { get; }
        #endregion

        [HttpPost("add")]
        public async Task<int> Add([FromBody] Auction Auction)
        {
            var AuctionId = await this.AuctionApplication.Add(Auction);
            return AuctionId;
        }

        [HttpPut("update")]
        public async Task<bool> Update([FromBody] Auction Auction)
        {
            var response = await AuctionApplication.Update(Auction);
            return response;
        }

        [HttpPut("activate")]
        public async Task<bool> Activate([FromBody] Auction Auction)
        {
            var response = await AuctionApplication.Activate(Auction);
            return response;
        }

        [HttpGet("get")]
        public async Task<Auction> Get([FromQuery] Auction request)
        {
            Auction response = await this.AuctionApplication.Get(request);
            return response;
        }

        [HttpGet("getlist")]
        public async Task<List<Auction>> GetList([FromQuery] Auction request)
        {
            List<Auction> response = await this.AuctionApplication.GetList(request);
            return response;
        }
        [HttpGet("gettimebox")]
        public async Task<AuctionTimebox> GetTimebox([FromQuery] Auction request)
        {
            var response = await this.AuctionApplication.GetTimebox(request);
            return response;
        }
    }
}
