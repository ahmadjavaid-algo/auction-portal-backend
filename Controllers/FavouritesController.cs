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
    [Authorize] // JWT required
    public class FavouritesController : APIBaseController
    {
        #region Constructor

        public FavouritesController(
            IFavouriteApplication favouriteApplication,
            IHeaderValue headerValue,
            IConfiguration configuration)
            : base(headerValue, configuration)
        {
            this.FavouriteApplication = favouriteApplication;
        }

        #endregion

        #region Properties and Data Members

        public IFavouriteApplication FavouriteApplication { get; }

        #endregion

        #region Helpers

        private int GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimsConstants.UserIdClaimType)
                        ?? User.FindFirst(ClaimTypes.NameIdentifier);

            int id;
            if (claim != null && int.TryParse(claim.Value, out id))
            {
                return id;
            }

            return 0;
        }

        #endregion

        #region Endpoints

        [HttpPost("add")]
        public async Task<int> Add([FromBody] Favourite favourite)
        {
            var userId = GetCurrentUserId();

            var favouriteId = await this.FavouriteApplication.AddForUser(favourite, userId);

            return favouriteId;
        }

        [HttpPut("update")]
        public async Task<bool> Update([FromBody] Favourite favourite)
        {
            var response = await this.FavouriteApplication.Update(favourite);
            return response;
        }

        [HttpPut("activate")]
        public async Task<bool> Activate([FromBody] Favourite favourite)
        {
            var userId = GetCurrentUserId();

            var response = await this.FavouriteApplication.ToggleActiveForUser(favourite, userId);

            return response;
        }

        [HttpGet("get")]
        public async Task<Favourite> Get([FromQuery] Favourite request)
        {
            Favourite response = await this.FavouriteApplication.Get(request);
            return response;
        }

        [HttpGet("getlist")]
        public async Task<List<Favourite>> GetList([FromQuery] Favourite request)
        {
            List<Favourite> response = await this.FavouriteApplication.GetList(request);
            return response;
        }

        #endregion
    }
}
