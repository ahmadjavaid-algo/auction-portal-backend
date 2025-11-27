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
    public class InspectorsController : APIBaseController
    {
        #region Constructor

        public InspectorsController(
            IInspectorApplication inspectorApplication,
            IHeaderValue headerValue,
            IConfiguration configuration,
            IAdminNotificationApplication adminNotificationApplication,
            IHubContext<NotificationHub> hubContext)
            : base(headerValue, configuration)
        {
            InspectorApplication = inspectorApplication
                ?? throw new ArgumentNullException(nameof(inspectorApplication));

            _adminNotificationApplication = adminNotificationApplication;
            _hubContext = hubContext;
        }

        #endregion

        #region Properties and Data Members

        public IInspectorApplication InspectorApplication { get; }

        private readonly IAdminNotificationApplication _adminNotificationApplication;
        private readonly IHubContext<NotificationHub> _hubContext;

        #endregion

        [HttpPost("add")]
        public async Task<int> Add([FromBody] Inspector inspector)
        {
            var inspectorId = await InspectorApplication.Add(inspector);
            return inspectorId;
        }

        [HttpPut("update")]
        public async Task<bool> Update([FromBody] Inspector inspector)
        {
            var response = await InspectorApplication.Update(inspector);
            return response;
        }

        [HttpPut("activate")]
        public async Task<bool> Activate([FromBody] Inspector inspector)
        {
            var response = await InspectorApplication.Activate(inspector);
            return response;
        }

        [HttpGet("get")]
        public async Task<Inspector> Get([FromQuery] Inspector request)
        {
            var response = await InspectorApplication.Get(request);
            return response;
        }

        [HttpGet("getlist")]
        public async Task<List<Inspector>> GetList([FromQuery] Inspector request)
        {
            var response = await InspectorApplication.GetList(request);
            return response;
        }

        [HttpGet("getstats")]
        public async Task<Inspector> GetStats()
        {
            return await InspectorApplication.GetStats();
        }
    }
}
