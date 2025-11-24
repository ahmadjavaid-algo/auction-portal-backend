using AuctionPortal.ApplicationLayer.IApplication;
using AuctionPortal.Common.Controllers;
using AuctionPortal.Common.Core;
using AuctionPortal.Common.Models;
using AuctionPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuctionPortal.Controllers
{
    public class DashboardsController : APIBaseController
    {
        #region Constructor
        /// <summary>
        /// DashboardsController initializes class object.
        /// </summary>
        public DashboardsController(IDashboardApplication DashboardApplication, IHeaderValue headerValue, IConfiguration configuration)
            : base(headerValue, configuration)
        {
            this.DashboardApplication = DashboardApplication;
        }
        #endregion

        #region Properties and Data Members
        public IDashboardApplication DashboardApplication { get; }
        #endregion

        [HttpPost("add")]
        public async Task<int> Add([FromBody] Dashboard Dashboard)
        {
            var DashboardId = await this.DashboardApplication.Add(Dashboard);
            return DashboardId;
        }

        [HttpPut("update")]
        public async Task<bool> Update([FromBody] Dashboard Dashboard)
        {
            var response = await DashboardApplication.Update(Dashboard);
            return response;
        }

        [HttpPut("activate")]
        public async Task<bool> Activate([FromBody] Dashboard Dashboard)
        {
            var response = await DashboardApplication.Activate(Dashboard);
            return response;
        }

        [HttpGet("get")]
        public async Task<Dashboard> Get([FromQuery] Dashboard request)
        {
            Dashboard response = await this.DashboardApplication.Get(request);
            return response;
        }

        [HttpGet("getlist")]
        public async Task<List<Dashboard>> GetList([FromQuery] Dashboard request)
        {
            List<Dashboard> response = await this.DashboardApplication.GetList(request);
            return response;
        }
    }
}
