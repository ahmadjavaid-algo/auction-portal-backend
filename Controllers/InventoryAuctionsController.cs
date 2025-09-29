using AuctionPortal.ApplicationLayer.IApplication;
using AuctionPortal.Common.Controllers;
using AuctionPortal.Common.Core;
using AuctionPortal.Common.Models;
using AuctionPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuctionPortal.Controllers
{
    public class InventoryAuctionsController : APIBaseController
    {
        #region Constructor
        /// <summary>
        /// InventoryAuctionsController initializes class object.
        /// </summary>
        public InventoryAuctionsController(IInventoryAuctionApplication InventoryAuctionApplication, IHeaderValue headerValue, IConfiguration configuration)
            : base(headerValue, configuration)
        {
            this.InventoryAuctionApplication = InventoryAuctionApplication;
        }
        #endregion

        #region Properties and Data Members
        public IInventoryAuctionApplication InventoryAuctionApplication { get; }
        #endregion

        [HttpPost("add")]
        public async Task<int> Add([FromBody] InventoryAuction InventoryAuction)
        {
            var InventoryAuctionId = await this.InventoryAuctionApplication.Add(InventoryAuction);
            return InventoryAuctionId;
        }

        [HttpPut("update")]
        public async Task<bool> Update([FromBody] InventoryAuction InventoryAuction)
        {
            var response = await InventoryAuctionApplication.Update(InventoryAuction);
            return response;
        }

        [HttpPut("activate")]
        public async Task<bool> Activate([FromBody] InventoryAuction InventoryAuction)
        {
            var response = await InventoryAuctionApplication.Activate(InventoryAuction);
            return response;
        }

        [HttpGet("get")]
        public async Task<InventoryAuction> Get([FromQuery] InventoryAuction request)
        {
            InventoryAuction response = await this.InventoryAuctionApplication.Get(request);
            return response;
        }

        [HttpGet("getlist")]
        public async Task<List<InventoryAuction>> GetList([FromQuery] InventoryAuction request)
        {
            List<InventoryAuction> response = await this.InventoryAuctionApplication.GetList(request);
            return response;
        }
    }
}
