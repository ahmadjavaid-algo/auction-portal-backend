using AuctionPortal.ApplicationLayer.IApplication;
using AuctionPortal.Common.Controllers;
using AuctionPortal.Common.Core;
using AuctionPortal.Common.Models;
using AuctionPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuctionPortal.Controllers
{
    public class InventoryDocumentFilesController : APIBaseController
    {
        #region Constructor
        /// <summary>
        /// InventoryDocumentFilesController initializes class object.
        /// </summary>
        public InventoryDocumentFilesController(IInventoryDocumentFileApplication InventoryDocumentFileApplication, IHeaderValue headerValue, IConfiguration configuration)
            : base(headerValue, configuration)
        {
            this.InventoryDocumentFileApplication = InventoryDocumentFileApplication;
        }
        #endregion

        #region Properties and Data Members
        public IInventoryDocumentFileApplication InventoryDocumentFileApplication { get; }
        #endregion

        [HttpPost("add")]
        public async Task<int> Add([FromBody] InventoryDocumentFile InventoryDocumentFile)
        {
            var InventoryDocumentFileId = await this.InventoryDocumentFileApplication.Add(InventoryDocumentFile);
            return InventoryDocumentFileId;
        }

        [HttpPut("update")]
        public async Task<bool> Update([FromBody] InventoryDocumentFile InventoryDocumentFile)
        {
            var response = await InventoryDocumentFileApplication.Update(InventoryDocumentFile);
            return response;
        }

        [HttpPut("activate")]
        public async Task<bool> Activate([FromBody] InventoryDocumentFile InventoryDocumentFile)
        {
            var response = await InventoryDocumentFileApplication.Activate(InventoryDocumentFile);
            return response;
        }

        [HttpGet("get")]
        public async Task<InventoryDocumentFile> Get([FromQuery] InventoryDocumentFile request)
        {
            InventoryDocumentFile response = await this.InventoryDocumentFileApplication.Get(request);
            return response;
        }

        [HttpGet("getlist")]
        public async Task<List<InventoryDocumentFile>> GetList([FromQuery] InventoryDocumentFile request)
        {
            List<InventoryDocumentFile> response = await this.InventoryDocumentFileApplication.GetList(request);
            return response;
        }
    }
}
