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
    public class InspectorOperationController : APIBaseController
    {
        #region Constructor
        public InspectorOperationController(
            IInspectorOperationApplication inspectorOperationApplication,
            IHeaderValue headerValue,
            IConfiguration configuration) : base(headerValue, configuration)
        {
            this.InspectorOperationApplication = inspectorOperationApplication;
        }
        #endregion

        #region Properties and Data Members
        public IInspectorOperationApplication InspectorOperationApplication { get; }
        #endregion

        [HttpPost("add")]
        public async Task<int> Add([FromBody] InspectorOperation request)
        {
            return await this.InspectorOperationApplication.Add(request);
        }

        [HttpPut("update")]
        public async Task<bool> Update([FromBody] InspectorOperation request)
        {
            return await this.InspectorOperationApplication.Update(request);
        }

        [HttpPut("activate")]
        public async Task<bool> Activate([FromBody] InspectorOperation request)
        {
            return await this.InspectorOperationApplication.Activate(request);
        }

        [HttpGet("get")]
        public async Task<InspectorOperation> Get([FromQuery] InspectorOperation request)
        {
            return await this.InspectorOperationApplication.Get(request);
        }

        [HttpGet("getlist")]
        public async Task<List<InspectorOperation>> GetList([FromQuery] InspectorOperation request)
        {
            return await this.InspectorOperationApplication.GetList(request);
        }

        [HttpPost("login")]
        public async Task<InspectorOperation> Login([FromBody] InspectorOperation request)
        {
            return await this.InspectorOperationApplication.Login(request);
        }

        [HttpPost("logout")]
        public async Task<bool> Logout([FromBody] InspectorOperation request)
        {
            return await this.InspectorOperationApplication.Logout(request);
        }

        [HttpPost("changepassword")]
        public async Task<bool> ChangePassword([FromBody] InspectorOperation request)
        {
            return await this.InspectorOperationApplication.ChangePassword(request);
        }

        [HttpPost("forgotpassword")]
        public async Task<bool> ForgotPassword([FromBody] InspectorOperation request)
        {
            return await this.InspectorOperationApplication.ForgotPassword(request);
        }

        [HttpPost("resetpassword")]
        public async Task<bool> ResetPassword([FromBody] InspectorOperation request)
        {
            return await this.InspectorOperationApplication.ResetPassword(request);
        }
    }
}
