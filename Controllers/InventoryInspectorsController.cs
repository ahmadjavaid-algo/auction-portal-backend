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
    public class InventoryInspectorsController : APIBaseController
    {
        #region Constructor

        public InventoryInspectorsController(
            IInventoryInspectorApplication inventoryInspectorApplication,
            IHeaderValue headerValue,
            IConfiguration configuration,
            IAdminNotificationApplication adminNotificationApplication,
            IHubContext<NotificationHub> hubContext)
            : base(headerValue, configuration)
        {
            InventoryInspectorApplication = inventoryInspectorApplication
                ?? throw new ArgumentNullException(nameof(InventoryInspectorApplication));

            _adminNotificationApplication = adminNotificationApplication;
            _hubContext = hubContext;
        }

        #endregion

        #region Properties and Data Members

        public IInventoryInspectorApplication InventoryInspectorApplication { get; }

        private readonly IAdminNotificationApplication _adminNotificationApplication;
        private readonly IHubContext<NotificationHub> _hubContext;

        #endregion

        [HttpPost("add")]
        public async Task<int> Add([FromBody] InventoryInspector InventoryInspector)
        {
            var InventoryInspectorId = await InventoryInspectorApplication.Add(InventoryInspector);
            return InventoryInspectorId;
        }

        [HttpPut("update")]
        public async Task<bool> Update([FromBody] InventoryInspector InventoryInspector)
        {
            var response = await InventoryInspectorApplication.Update(InventoryInspector);
            return response;
        }

        [HttpPut("activate")]
        public async Task<bool> Activate([FromBody] InventoryInspector InventoryInspector)
        {
            var response = await InventoryInspectorApplication.Activate(InventoryInspector);
            return response;
        }

        [HttpGet("get")]
        public async Task<InventoryInspector> Get([FromQuery] InventoryInspector request)
        {
            var response = await InventoryInspectorApplication.Get(request);
            return response;
        }

        [HttpGet("getlist")]
        public async Task<List<InventoryInspector>> GetList([FromQuery] InventoryInspector request)
        {
            var response = await InventoryInspectorApplication.GetList(request);
            return response;
        }

    }
}
