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
    public class InspectionCheckpointsController : APIBaseController
    {
        #region Constructor

        public InspectionCheckpointsController(
            IInspectionCheckpointApplication inspectionCheckpointApplication,
            IHeaderValue headerValue,
            IConfiguration configuration,
            IAdminNotificationApplication adminNotificationApplication,
            IHubContext<NotificationHub> hubContext)
            : base(headerValue, configuration)
        {
            InspectionCheckpointApplication = inspectionCheckpointApplication
                ?? throw new ArgumentNullException(nameof(InspectionCheckpointApplication));

            _adminNotificationApplication = adminNotificationApplication;
            _hubContext = hubContext;
        }

        #endregion

        #region Properties and Data Members

        public IInspectionCheckpointApplication InspectionCheckpointApplication { get; }

        private readonly IAdminNotificationApplication _adminNotificationApplication;
        private readonly IHubContext<NotificationHub> _hubContext;

        #endregion

        [HttpPost("add")]
        public async Task<int> Add([FromBody] InspectionCheckpoint InspectionCheckpoint)
        {
            var InspectionCheckpointId = await InspectionCheckpointApplication.Add(InspectionCheckpoint);
            return InspectionCheckpointId;
        }

        [HttpPut("update")]
        public async Task<bool> Update([FromBody] InspectionCheckpoint InspectionCheckpoint)
        {
            var response = await InspectionCheckpointApplication.Update(InspectionCheckpoint);
            return response;
        }

        [HttpPut("activate")]
        public async Task<bool> Activate([FromBody] InspectionCheckpoint InspectionCheckpoint)
        {
            var response = await InspectionCheckpointApplication.Activate(InspectionCheckpoint);
            return response;
        }

        [HttpGet("get")]
        public async Task<InspectionCheckpoint> Get([FromQuery] InspectionCheckpoint request)
        {
            var response = await InspectionCheckpointApplication.Get(request);
            return response;
        }

        [HttpGet("getlist")]
        public async Task<List<InspectionCheckpoint>> GetList([FromQuery] InspectionCheckpoint request)
        {
            var response = await InspectionCheckpointApplication.GetList(request);
            return response;
        }

    }
}
