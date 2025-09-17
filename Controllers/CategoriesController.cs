using AuctionPortal.ApplicationLayer.IApplication;
using AuctionPortal.Common.Controllers;
using AuctionPortal.Common.Core;
using AuctionPortal.Common.Models;
using AuctionPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuctionPortal.Controllers
{
    public class CategoriesController : APIBaseController
    {
        #region Constructor
        /// <summary>
        /// CategorysController initializes class object.
        /// </summary>
        public CategoriesController(ICategoryApplication CategoryApplication, IHeaderValue headerValue, IConfiguration configuration)
            : base(headerValue, configuration)
        {
            this.CategoryApplication = CategoryApplication;
        }
        #endregion

        #region Properties and Data Members
        public ICategoryApplication CategoryApplication { get; }
        #endregion

        [HttpPost("add")]
        public async Task<int> Add([FromBody] Category Category)
        {
            var CategoryId = await this.CategoryApplication.Add(Category);
            return CategoryId;
        }

        [HttpPut("update")]
        public async Task<bool> Update([FromBody] Category Category)
        {
            var response = await CategoryApplication.Update(Category);
            return response;
        }

        [HttpPut("activate")]
        public async Task<bool> Activate([FromBody] Category Category)
        {
            var response = await CategoryApplication.Activate(Category);
            return response;
        }

        [HttpGet("get")]
        public async Task<Category> Get([FromQuery] Category request)
        {
            Category response = await this.CategoryApplication.Get(request);
            return response;
        }

        [HttpGet("getlist")]
        public async Task<List<Category>> GetList([FromQuery] Category request)
        {
            List<Category> response = await this.CategoryApplication.GetList(request);
            return response;
        }
    }
}
