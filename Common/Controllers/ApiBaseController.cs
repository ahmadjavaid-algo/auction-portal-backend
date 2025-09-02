using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using AuctionPortal.Common.Core; // IHeaderValue

namespace AuctionPortal.Common.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class APIBaseController : ControllerBase
    {
        #region Constructor
        /// <summary>
        /// APIBaseController initializes class object.
        /// </summary>
        /// <param name="headerValue"></param>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        public APIBaseController(IHeaderValue headerValue, IConfiguration configuration)
        {
            this.HeaderValue = headerValue;
            this.Configuration = configuration;
        }
        #endregion

        #region Properties and Data Members
        protected IHeaderValue HeaderValue { get; }
        public IConfiguration Configuration { get; }
        #endregion

        #region API Methods

        /// <summary>
        /// Heartbeatprovides API to return Test Message.
        /// API Path:  api/ControllerName/Heartbeat
        /// </summary>
        /// <param name="requestVm"></param>
        /// <returns></returns>
        [HttpGet("Heartbeat")]
        public string Heartbeat()
        {
            return "Hello Test at: " + DateTime.Now;
        }

        #endregion



        #region SignalR Group Names
        public const string DealerGroupName = "DealerGroup";
        public const string AdminGroupName = "AdminGroup";
        #endregion
    }
}
