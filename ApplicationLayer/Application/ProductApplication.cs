using AuctionPortal.ApplicationLayer.IApplication;
using AuctionPortal.InfrastructureLayer.Interfaces;
using AuctionPortal.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuctionPortal.ApplicationLayer.Application
{
    public class ProductApplication : BaseApplication, IProductApplication
    {
        public ProductApplication(IProductInfrastructure ProductInfrastructure, IConfiguration configuration)
            : base(configuration)
        {
            this.ProductInfrastructure = ProductInfrastructure ?? throw new ArgumentNullException(nameof(ProductInfrastructure));
        }

        public IProductInfrastructure ProductInfrastructure { get; }

        #region Queries
        public async Task<Product> Get(Product entity)
        {
            return await ProductInfrastructure.Get(entity);
        }

        public async Task<List<Product>> GetList(Product entity)
        {
            return await ProductInfrastructure.GetList(entity);
        }
        #endregion

        #region Commands
        public async Task<int> Add(Product entity)
        {
            return await ProductInfrastructure.Add(entity);
        }

        public async Task<bool> Update(Product entity)
        {
            return await ProductInfrastructure.Update(entity);
        }

        public async Task<bool> Activate(Product entity)
        {
            return await ProductInfrastructure.Activate(entity);
        }
        #endregion
    }
}
