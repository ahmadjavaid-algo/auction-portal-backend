using AuctionPortal.ApplicationLayer.IApplication;
using AuctionPortal.InfrastructureLayer.Interfaces;
using AuctionPortal.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuctionPortal.ApplicationLayer.Application
{
    public class ModelApplication : BaseApplication, IModelApplication
    {
        public ModelApplication(IModelInfrastructure ModelInfrastructure, IConfiguration configuration)
            : base(configuration)
        {
            this.ModelInfrastructure = ModelInfrastructure ?? throw new ArgumentNullException(nameof(ModelInfrastructure));
        }

        public IModelInfrastructure ModelInfrastructure { get; }

        #region Queries
        public async Task<Model> Get(Model entity)
        {
            return await ModelInfrastructure.Get(entity);
        }

        public async Task<List<Model>> GetList(Model entity)
        {
            return await ModelInfrastructure.GetList(entity);
        }
        #endregion

        #region Commands
        public async Task<int> Add(Model entity)
        {
            return await ModelInfrastructure.Add(entity);
        }

        public async Task<bool> Update(Model entity)
        {
            return await ModelInfrastructure.Update(entity);
        }

        public async Task<bool> Activate(Model entity)
        {
            return await ModelInfrastructure.Activate(entity);
        }
        #endregion
    }
}
