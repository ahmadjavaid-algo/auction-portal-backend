using AuctionPortal.ApplicationLayer.IApplication;
using AuctionPortal.Common.Controllers;
using AuctionPortal.Common.Core;
using AuctionPortal.Common.Models;
using AuctionPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuctionPortal.Controllers
{
    public class FavouritesController : APIBaseController
    {
        #region Constructor
        /// <summary>
        /// FavouritesController initializes class object.
        /// </summary>
        public FavouritesController(IFavouriteApplication FavouriteApplication, IHeaderValue headerValue, IConfiguration configuration)
            : base(headerValue, configuration)
        {
            this.FavouriteApplication = FavouriteApplication;
        }
        #endregion

        #region Properties and Data Members
        public IFavouriteApplication FavouriteApplication { get; }
        #endregion

        [HttpPost("add")]
        public async Task<int> Add([FromBody] Favourite Favourite)
        {
            var FavouriteId = await this.FavouriteApplication.Add(Favourite);
            return FavouriteId;
        }

        [HttpPut("update")]
        public async Task<bool> Update([FromBody] Favourite Favourite)
        {
            var response = await FavouriteApplication.Update(Favourite);
            return response;
        }

        [HttpPut("activate")]
        public async Task<bool> Activate([FromBody] Favourite Favourite)
        {
            var response = await FavouriteApplication.Activate(Favourite);
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
    }
}
