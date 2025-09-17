using AuctionPortal.ApplicationLayer.IApplication;
using AuctionPortal.Common.Controllers;
using AuctionPortal.Common.Core;
using AuctionPortal.Common.Models;
using AuctionPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuctionPortal.Controllers
{
    public class YearsController : APIBaseController
    {
        #region Constructor
        /// <summary>
        /// YearsController initializes class object.
        /// </summary>
        public YearsController(IYearApplication YearApplication, IHeaderValue headerValue, IConfiguration configuration)
            : base(headerValue, configuration)
        {
            this.YearApplication = YearApplication;
        }
        #endregion

        #region Properties and Data Members
        public IYearApplication YearApplication { get; }
        #endregion

        [HttpPost("add")]
        public async Task<int> Add([FromBody] Year Year)
        {
            var YearId = await this.YearApplication.Add(Year);
            return YearId;
        }

        [HttpPut("update")]
        public async Task<bool> Update([FromBody] Year Year)
        {
            var response = await YearApplication.Update(Year);
            return response;
        }

        [HttpPut("activate")]
        public async Task<bool> Activate([FromBody] Year Year)
        {
            var response = await YearApplication.Activate(Year);
            return response;
        }

        [HttpGet("get")]
        public async Task<Year> Get([FromQuery] Year request)
        {
            Year response = await this.YearApplication.Get(request);
            return response;
        }

        [HttpGet("getlist")]
        public async Task<List<Year>> GetList([FromQuery] Year request)
        {
            List<Year> response = await this.YearApplication.GetList(request);
            return response;
        }
    }
}
