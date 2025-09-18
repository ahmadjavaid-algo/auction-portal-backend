using AuctionPortal.ApplicationLayer.IApplication;
using AuctionPortal.Common.Controllers;
using AuctionPortal.Common.Core;
using AuctionPortal.Common.Models;
using AuctionPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuctionPortal.Controllers
{
    public class InventorysController : APIBaseController
    {
        #region Constructor
        /// <summary>
        /// InventorysController initializes class object.
        /// </summary>
        public InventorysController(IInventoryApplication InventoryApplication, IHeaderValue headerValue, IConfiguration configuration)
            : base(headerValue, configuration)
        {
            this.InventoryApplication = InventoryApplication;
        }
        #endregion

        #region Properties and Data Members
        public IInventoryApplication InventoryApplication { get; }
        #endregion

        [HttpPost("add")]
        public async Task<int> Add([FromBody] Inventory Inventory)
        {
            var InventoryId = await this.InventoryApplication.Add(Inventory);
            return InventoryId;
        }

        [HttpPut("update")]
        public async Task<bool> Update([FromBody] Inventory Inventory)
        {
            var response = await InventoryApplication.Update(Inventory);
            return response;
        }

        [HttpPut("activate")]
        public async Task<bool> Activate([FromBody] Inventory Inventory)
        {
            var response = await InventoryApplication.Activate(Inventory);
            return response;
        }

        [HttpGet("get")]
        public async Task<Inventory> Get([FromQuery] Inventory request)
        {
            Inventory response = await this.InventoryApplication.Get(request);
            return response;
        }

        [HttpGet("getlist")]
        public async Task<List<Inventory>> GetList([FromQuery] Inventory request)
        {
            List<Inventory> response = await this.InventoryApplication.GetList(request);
            return response;
        }
    }
}
