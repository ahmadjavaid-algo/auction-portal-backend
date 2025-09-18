using AuctionPortal.ApplicationLayer.IApplication;
using AuctionPortal.Common.Controllers;
using AuctionPortal.Common.Core;
using AuctionPortal.Common.Models;
using AuctionPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuctionPortal.Controllers
{
    public class ProductsController : APIBaseController
    {
        #region Constructor
        /// <summary>
        /// ProductsController initializes class object.
        /// </summary>
        public ProductsController(IProductApplication ProductApplication, IHeaderValue headerValue, IConfiguration configuration)
            : base(headerValue, configuration)
        {
            this.ProductApplication = ProductApplication;
        }
        #endregion

        #region Properties and Data Members
        public IProductApplication ProductApplication { get; }
        #endregion

        [HttpPost("add")]
        public async Task<int> Add([FromBody] Product Product)
        {
            var ProductId = await this.ProductApplication.Add(Product);
            return ProductId;
        }

        [HttpPut("update")]
        public async Task<bool> Update([FromBody] Product Product)
        {
            var response = await ProductApplication.Update(Product);
            return response;
        }

        [HttpPut("activate")]
        public async Task<bool> Activate([FromBody] Product Product)
        {
            var response = await ProductApplication.Activate(Product);
            return response;
        }

        [HttpGet("get")]
        public async Task<Product> Get([FromQuery] Product request)
        {
            Product response = await this.ProductApplication.Get(request);
            return response;
        }

        [HttpGet("getlist")]
        public async Task<List<Product>> GetList([FromQuery] Product request)
        {
            List<Product> response = await this.ProductApplication.GetList(request);
            return response;
        }
    }
}
