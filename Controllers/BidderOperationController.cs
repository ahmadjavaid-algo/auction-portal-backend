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
    public class BidderOperationController : APIBaseController
    {
        #region Constructor
        public BidderOperationController(
            IBidderOperationApplication BidderOperationApplication,
            IHeaderValue headerValue,
            IConfiguration configuration) : base(headerValue, configuration)
        {
            this.BidderOperationApplication = BidderOperationApplication;
        }
        #endregion

        #region Properties and Data Members
        public IBidderOperationApplication BidderOperationApplication { get; }
        #endregion

        [HttpPost("add")]
        public async Task<int> Add([FromBody] BidderOperation request)
        {
            return await this.BidderOperationApplication.Add(request);
        }

        [HttpPut("update")]
        public async Task<bool> Update([FromBody] BidderOperation request)
        {
            return await this.BidderOperationApplication.Update(request);
        }

        [HttpPut("activate")]
        public async Task<bool> Activate([FromBody] BidderOperation request)
        {
            return await this.BidderOperationApplication.Activate(request);
        }

        [HttpGet("get")]
        public async Task<BidderOperation> Get([FromQuery] BidderOperation request)
        {
            return await this.BidderOperationApplication.Get(request);
        }

        [HttpGet("getlist")]
        public async Task<List<BidderOperation>> GetList([FromQuery] BidderOperation request)
        {
            return await this.BidderOperationApplication.GetList(request);
        }

        [HttpPost("login")]
        public async Task<BidderOperation> Login([FromBody] BidderOperation request)
        {
            return await this.BidderOperationApplication.Login(request);
        }

        [HttpPost("logout")]
        public async Task<bool> Logout([FromBody] BidderOperation request)
        {
            return await this.BidderOperationApplication.Logout(request);
        }

        [HttpPost("changepassword")]
        public async Task<bool> ChangePassword([FromBody] BidderOperation request)
        {
            return await this.BidderOperationApplication.ChangePassword(request);
        }

        [HttpPost("forgotpassword")]
        public async Task<bool> ForgotPassword([FromBody] BidderOperation request)
        {
            return await this.BidderOperationApplication.ForgotPassword(request);
        }

        [HttpPost("resetpassword")]
        public async Task<bool> ResetPassword([FromBody] BidderOperation request)
        {
            return await this.BidderOperationApplication.ResetPassword(request);
        }
    }
}
