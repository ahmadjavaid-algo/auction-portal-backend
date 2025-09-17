using AuctionPortal.ApplicationLayer.IApplication;
using AuctionPortal.Common.Controllers;
using AuctionPortal.Common.Core;
using AuctionPortal.Common.Models;
using AuctionPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuctionPortal.Controllers
{
    public class MakesController : APIBaseController
    {
        #region Constructor
        /// <summary>
        /// MakesController initializes class object.
        /// </summary>
        public MakesController(IMakeApplication makeApplication, IHeaderValue headerValue, IConfiguration configuration)
            : base(headerValue, configuration)
        {
            this.MakeApplication = makeApplication;
        }
        #endregion

        #region Properties and Data Members
        public IMakeApplication MakeApplication { get; }
        #endregion

        [HttpPost("add")]
        public async Task<int> Add([FromBody] Make make)
        {
            var makeId = await this.MakeApplication.Add(make);
            return makeId;
        }

        [HttpPut("update")]
        public async Task<bool> Update([FromBody] Make make)
        {
            var response = await MakeApplication.Update(make);
            return response;
        }

        [HttpPut("activate")]
        public async Task<bool> Activate([FromBody] Make make)
        {
            var response = await MakeApplication.Activate(make);
            return response;
        }

        [HttpGet("get")]
        public async Task<Make> Get([FromQuery] Make request)
        {
            Make response = await this.MakeApplication.Get(request);
            return response;
        }

        [HttpGet("getlist")]
        public async Task<List<Make>> GetList([FromQuery] Make request)
        {
            List<Make> response = await this.MakeApplication.GetList(request);
            return response;
        }
    }
}
