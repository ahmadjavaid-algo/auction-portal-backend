using AuctionPortal.ApplicationLayer.Application;
using AuctionPortal.ApplicationLayer.IApplication;
using AuctionPortal.Common.Controllers;
using AuctionPortal.Common.Core;
using AuctionPortal.Common.Models;
using AuctionPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuctionPortal.Controllers
{
    public class UsersController : APIBaseController
    {
        #region Constructor
        /// <summary>
        /// UsersController initializes class object.
        /// </summary>
        public UsersController(IUserApplication userApplication, IHeaderValue headerValue, IConfiguration configuration)
            : base(headerValue, configuration)
        {
            this.UserApplication = userApplication;
        }
        #endregion

        #region Properties and Data Members
        public IUserApplication UserApplication { get; }
        #endregion

        [HttpPost("add")]
        [Authorize(Policy = "USER_UAD")]
        public async Task<int> Add([FromBody] User user)
        {
            var applicationRoleId = await this.UserApplication.Add(user);
            return applicationRoleId;
        }

        [HttpPut("update")]
        [Authorize(Policy = "USER_UUP")]
        public async Task<bool> Update([FromBody] User user)
        {
            var response = await this.UserApplication.Update(user);
            return response;
        }

        [HttpPut("activate")]
        [Authorize(Policy = "USER_UAC")]
        public async Task<bool> Activate([FromBody] User user)
        {
            var response = await this.UserApplication.Activate(user);
            return response;
        }

        [HttpGet("get")]
        [Authorize(Policy = "USER_UGI")]
        public async Task<User> Get([FromQuery] User request)
        {
            var response = await this.UserApplication.Get(request);
            return response;
        }

        [HttpGet("getlist")]
        [Authorize(Policy = "USER_UGL")]
        public async Task<List<User>> GetList([FromQuery] User request)
        {
            var response = await this.UserApplication.GetList(request);
            return response;
        }
        [HttpGet("getstats")]
        public async Task<User> GetStats()
        {
            return await this.UserApplication.GetStats();
        }
    }
}
