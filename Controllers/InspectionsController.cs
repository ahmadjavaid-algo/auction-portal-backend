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
    public class InspectionsController : APIBaseController
    {
        #region Constructor

        public InspectionsController(
            IInspectionApplication inspectionApplication,
            IHeaderValue headerValue,
            IConfiguration configuration,
            IAdminNotificationApplication adminNotificationApplication,
            IHubContext<NotificationHub> hubContext)
            : base(headerValue, configuration)
        {
            InspectionApplication = inspectionApplication
                ?? throw new ArgumentNullException(nameof(InspectionApplication));

            _adminNotificationApplication = adminNotificationApplication;
            _hubContext = hubContext;
        }

        #endregion

        #region Properties and Data Members

        public IInspectionApplication InspectionApplication { get; }

        private readonly IAdminNotificationApplication _adminNotificationApplication;
        private readonly IHubContext<NotificationHub> _hubContext;

        #endregion

        [HttpPost("add")]
        public async Task<int> Add([FromBody] Inspection Inspection)
        {
            var InspectionId = await InspectionApplication.Add(Inspection);
            return InspectionId;
        }

        [HttpPut("update")]
        public async Task<bool> Update([FromBody] Inspection Inspection)
        {
            var response = await InspectionApplication.Update(Inspection);
            return response;
        }

        [HttpPut("activate")]
        public async Task<bool> Activate([FromBody] Inspection Inspection)
        {
            var response = await InspectionApplication.Activate(Inspection);
            return response;
        }

        [HttpGet("get")]
        public async Task<Inspection> Get([FromQuery] Inspection request)
        {
            var response = await InspectionApplication.Get(request);
            return response;
        }

        [HttpGet("getlist")]
        public async Task<List<Inspection>> GetList([FromQuery] Inspection request)
        {
            var response = await InspectionApplication.GetList(request);
            return response;
        }

        [HttpGet("getbyinventory")]
        public async Task<List<Inspection>> GetByInventory([FromQuery] Inspection request)
        {
            var response = await InspectionApplication.GetByInventory(request);
            return response;
        }
    }
}
