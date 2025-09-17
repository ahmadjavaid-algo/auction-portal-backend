using AuctionPortal.ApplicationLayer.IApplication;
using AuctionPortal.InfrastructureLayer.Interfaces;
using AuctionPortal.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuctionPortal.ApplicationLayer.Application
{
    public class CategoryApplication : BaseApplication, ICategoryApplication
    {
        public CategoryApplication(ICategoryInfrastructure CategoryInfrastructure, IConfiguration configuration)
            : base(configuration)
        {
            this.CategoryInfrastructure = CategoryInfrastructure ?? throw new ArgumentNullException(nameof(CategoryInfrastructure));
        }

        public ICategoryInfrastructure CategoryInfrastructure { get; }

        #region Queries
        public async Task<Category> Get(Category entity)
        {
            return await CategoryInfrastructure.Get(entity);
        }

        public async Task<List<Category>> GetList(Category entity)
        {
            return await CategoryInfrastructure.GetList(entity);
        }
        #endregion

        #region Commands
        public async Task<int> Add(Category entity)
        {
            return await CategoryInfrastructure.Add(entity);
        }

        public async Task<bool> Update(Category entity)
        {
            return await CategoryInfrastructure.Update(entity);
        }

        public async Task<bool> Activate(Category entity)
        {
            return await CategoryInfrastructure.Activate(entity);
        }
        #endregion
    }
}
