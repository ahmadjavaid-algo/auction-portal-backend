using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AuctionPortal.ApplicationLayer.IApplication;
using AuctionPortal.Common.Auth;
using AuctionPortal.Common.Controllers;
using AuctionPortal.Common.Core;
using AuctionPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuctionPortal.Controllers
{
    
    public class AuctionBidsController : APIBaseController
    {
        #region Constructor

        public AuctionBidsController(
            IAuctionBidApplication AuctionBidApplication,
            IHeaderValue headerValue,
            IConfiguration configuration)
            : base(headerValue, configuration)
        {
            this.AuctionBidApplication = AuctionBidApplication;
        }

        #endregion

        #region Properties and Data Members

        public IAuctionBidApplication AuctionBidApplication { get; }

        #endregion

        #region Helpers

        private int GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimsConstants.UserIdClaimType)
                        ?? User.FindFirst(ClaimTypes.NameIdentifier);

            int id;
            return claim != null && int.TryParse(claim.Value, out id) ? id : 0;
        }

        #endregion

        #region Endpoints

        [HttpPost("add")]
        public async Task<int> Add([FromBody] AuctionBid AuctionBid)
        {
            var userId = GetCurrentUserId();
            if (userId <= 0)
            {
                return 0;
            }

            
            AuctionBid.CreatedById = userId;

            var newId = await AuctionBidApplication.Add(AuctionBid);
            return newId;
        }

        [HttpPut("update")]
        public async Task<bool> Update([FromBody] AuctionBid AuctionBid)
        {
            var response = await AuctionBidApplication.Update(AuctionBid);
            return response;
        }

        [HttpPut("activate")]
        public async Task<bool> Activate([FromBody] AuctionBid AuctionBid)
        {
            var response = await AuctionBidApplication.Activate(AuctionBid);
            return response;
        }

        [HttpGet("get")]
        public async Task<AuctionBid> Get([FromQuery] AuctionBid request)
        {
            var response = await AuctionBidApplication.Get(request);
            return response;
        }

        [HttpGet("getlist")]
        public async Task<List<AuctionBid>> GetList([FromQuery] AuctionBid request)
        {
            var response = await AuctionBidApplication.GetList(request);
            return response;
        }

        #endregion
    }
}
