using AuctionPortal.ApplicationLayer.IApplication;
using AuctionPortal.Common.Controllers;
using AuctionPortal.Common.Core;
using AuctionPortal.Common.Models;
using AuctionPortal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuctionPortal.Controllers
{
    public class ApplicationUserOperationsController : APIBaseController
    {
        #region Constructor
        public ApplicationUserOperationsController(
            IApplicationUserOperationApplication ApplicationUserOperationApplication,
            IHeaderValue headerValue,
            IConfiguration configuration) : base(headerValue, configuration)
        {
            this.ApplicationUserOperationApplication = ApplicationUserOperationApplication;
        }
        #endregion

        #region Properties and Data Members
        public IApplicationUserOperationApplication ApplicationUserOperationApplication { get; }
        #endregion

        [HttpPost("add")]
        public async Task<int> Add([FromBody] ApplicationUserOperation request)
        {
            return await this.ApplicationUserOperationApplication.Add(request);
        }

        [HttpPut("update")]
        public async Task<bool> Update([FromBody] ApplicationUserOperation request)
        {
            return await this.ApplicationUserOperationApplication.Update(request);
        }

        [HttpPut("activate")]
        public async Task<bool> Activate([FromBody] ApplicationUserOperation request)
        {
            return await this.ApplicationUserOperationApplication.Activate(request);
        }

        [HttpGet("get")]
        public async Task<ApplicationUserOperation> Get([FromQuery] ApplicationUserOperation request)
        {
            return await this.ApplicationUserOperationApplication.Get(request);
        }

        [HttpGet("getlist")]
        public async Task<List<ApplicationUserOperation>> GetList([FromQuery] ApplicationUserOperation request)
        {
            return await this.ApplicationUserOperationApplication.GetList(request);
        }

        [HttpPost("login")]
        public async Task<ApplicationUserOperation> Login([FromBody] ApplicationUserOperation request)
        {
            return await this.ApplicationUserOperationApplication.Login(request);
        }

        [HttpPost("logout")]
        public async Task<bool> Logout([FromBody] ApplicationUserOperation request)
        {
            return await this.ApplicationUserOperationApplication.Logout(request);
        }

        [HttpPost("changepassword")]
        public async Task<bool> ChangePassword([FromBody] ApplicationUserOperation request)
        {
            return await this.ApplicationUserOperationApplication.ChangePassword(request);
        }

        [HttpPost("forgotpassword")]
        public async Task<bool> ForgotPassword([FromBody] ApplicationUserOperation request)
        {
            return await this.ApplicationUserOperationApplication.ForgotPassword(request);
        }

        [HttpPost("resetpassword")]
        public async Task<bool> ResetPassword([FromBody] ApplicationUserOperation request)
        {
            return await this.ApplicationUserOperationApplication.ResetPassword(request);
        }
    }
}
