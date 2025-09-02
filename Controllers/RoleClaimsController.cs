// Controllers/RoleClaimsController.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using AuctionPortal.ApplicationLayer.IApplication;
using AuctionPortal.Common.Controllers;
using AuctionPortal.Common.Core;
using AuctionPortal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace AuctionPortal.Controllers
{
    public class RoleClaimsController : APIBaseController
    {
        public IClaimApplication RoleClaimApplication { get; }

        public RoleClaimsController(
            IClaimApplication roleClaimApplication,
            IHeaderValue headerValue,
            IConfiguration configuration)
            : base(headerValue, configuration)
        {
            RoleClaimApplication = roleClaimApplication;
        }

        [HttpGet("byrole")]
        public async Task<List<RoleClaims>> GetByRole([FromQuery] RoleClaims request)
        {
            return await RoleClaimApplication.GetByRole(request);
        }

        [HttpPost("set")]
        public async Task<bool> SetForRole([FromBody] RoleClaims request)
        {
            return await RoleClaimApplication.SetForRole(request);
        }

        [HttpPost("add")]
        public async Task<bool> Add([FromBody] RoleClaims request)
        {
            return await RoleClaimApplication.Add(request);
        }

        [HttpPost("remove")]
        public async Task<bool> Remove([FromBody] RoleClaims request)
        {
            return await RoleClaimApplication.Remove(request);
        }
    }
}
