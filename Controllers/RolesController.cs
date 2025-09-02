using AuctionPortal.ApplicationLayer.IApplication;
using AuctionPortal.Common.Controllers;
using AuctionPortal.Common.Core;
using AuctionPortal.Common.Models;
using AuctionPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuctionPortal.Controllers
{
  
    public class RolesController : APIBaseController
    {
        #region Constructor
        /// <summary>
        /// RolesController initializes class object.
        /// </summary>
        public RolesController(IRoleApplication roleApplication, IHeaderValue headerValue, IConfiguration configuration)
            : base(headerValue, configuration)
        {
            this.RoleApplication = roleApplication;
        }
        #endregion

        #region Properties and Data Members
        public IRoleApplication RoleApplication { get; }
        #endregion

        [HttpPost("add")]
        [Authorize(Policy = "ROLE_RAD")] 
        public async Task<int> Add([FromBody] Role role)
        {
            var applicationRoleId = await this.RoleApplication.Add(role);
            return applicationRoleId;
        }

        [HttpPut("update")]
        [Authorize(Policy = "ROLE_RUP")] 
        public async Task<bool> Update([FromBody] Role role)
        {
            var response = await RoleApplication.Update(role);
            return response;
        }

        [HttpPut("activate")]
        [Authorize(Policy = "ROLE_RAC")] 
        public async Task<bool> Activate([FromBody] Role role)
        {
            var response = await RoleApplication.Activate(role);
            return response;
        }

        [HttpGet("get")]
        [Authorize(Policy = "ROLE_RGI")] 
        public async Task<Role> Get([FromQuery] Role request)
        {
            Role response = await this.RoleApplication.Get(request);
            return response;
        }

        [HttpGet("getlist")]
        [Authorize(Policy = "ROLE_RGL")] 
        public async Task<List<Role>> GetList([FromQuery] Role request)
        {
            List<Role> response = await this.RoleApplication.GetList(request);
            return response;
        }
    }
}
