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
    public class InspectionTypesController : APIBaseController
    {
        #region Constructor

        public InspectionTypesController(
            IInspectionTypeApplication inspectionTypeApplication,
            IHeaderValue headerValue,
            IConfiguration configuration,
            IAdminNotificationApplication adminNotificationApplication,
            IHubContext<NotificationHub> hubContext)
            : base(headerValue, configuration)
        {
            InspectionTypeApplication = inspectionTypeApplication
                ?? throw new ArgumentNullException(nameof(InspectionTypeApplication));

            _adminNotificationApplication = adminNotificationApplication;
            _hubContext = hubContext;
        }

        #endregion

        #region Properties and Data Members

        public IInspectionTypeApplication InspectionTypeApplication { get; }

        private readonly IAdminNotificationApplication _adminNotificationApplication;
        private readonly IHubContext<NotificationHub> _hubContext;

        #endregion

        [HttpPost("add")]
        public async Task<int> Add([FromBody] InspectionType InspectionType)
        {
            var InspectionTypeId = await InspectionTypeApplication.Add(InspectionType);
            return InspectionTypeId;
        }

        [HttpPut("update")]
        public async Task<bool> Update([FromBody] InspectionType InspectionType)
        {
            var response = await InspectionTypeApplication.Update(InspectionType);
            return response;
        }

        [HttpPut("activate")]
        public async Task<bool> Activate([FromBody] InspectionType InspectionType)
        {
            var response = await InspectionTypeApplication.Activate(InspectionType);
            return response;
        }

        [HttpGet("get")]
        public async Task<InspectionType> Get([FromQuery] InspectionType request)
        {
            var response = await InspectionTypeApplication.Get(request);
            return response;
        }

        [HttpGet("getlist")]
        public async Task<List<InspectionType>> GetList([FromQuery] InspectionType request)
        {
            var response = await InspectionTypeApplication.GetList(request);
            return response;
        }

    }
}
