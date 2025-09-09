using AuctionPortal.ApplicationLayer.IApplication;
using AuctionPortal.Common.Controllers;
using AuctionPortal.Common.Core;
using AuctionPortal.Common.Models;
using AuctionPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuctionPortal.Controllers
{

    public class EmailsController : APIBaseController
    {
        #region Constructor
        /// <summary>
        /// EmailsController initializes class object.
        /// </summary>
        public EmailsController(IEmailsApplication EmailApplication, IHeaderValue headerValue, IConfiguration configuration)
            : base(headerValue, configuration)
        {
            this.EmailApplication = EmailApplication;
        }
        #endregion

        #region Properties and Data Members
        public IEmailsApplication EmailApplication { get; }
        #endregion

        [HttpPost("add")]
        public async Task<int> Add([FromBody] Email Email)
        {
            var applicationEmailId = await this.EmailApplication.Add(Email);
            return applicationEmailId;
        }

        [HttpPut("update")]
        public async Task<bool> Update([FromBody] Email Email)
        {
            var response = await EmailApplication.Update(Email);
            return response;
        }

        [HttpPut("activate")]
        public async Task<bool> Activate([FromBody] Email Email)
        {
            var response = await EmailApplication.Activate(Email);
            return response;
        }

        [HttpGet("get")]
        public async Task<Email> Get([FromQuery] Email request)
        {
            Email response = await this.EmailApplication.Get(request);
            return response;
        }

        [HttpGet("getlist")]
        public async Task<List<Email>> GetList([FromQuery] Email request)
        {
            List<Email> response = await this.EmailApplication.GetList(request);
            return response;
        }
    }
}
