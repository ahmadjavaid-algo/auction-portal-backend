// InventoryInspectorApplication.cs
using AuctionPortal.ApplicationLayer.IApplication;
using AuctionPortal.InfrastructureLayer.Interfaces;
using AuctionPortal.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuctionPortal.ApplicationLayer.Application
{
    public class InventoryInspectorApplication : BaseApplication, IInventoryInspectorApplication
    {
        public InventoryInspectorApplication(IInventoryInspectorInfrastructure InventoryInspectorInfrastructure, IConfiguration configuration)
            : base(configuration)
        {
            this.InventoryInspectorInfrastructure = InventoryInspectorInfrastructure
                ?? throw new ArgumentNullException(nameof(InventoryInspectorInfrastructure));
        }

        public IInventoryInspectorInfrastructure InventoryInspectorInfrastructure { get; }

        #region Queries
        public async Task<InventoryInspector> Get(InventoryInspector entity)
        {
            return await InventoryInspectorInfrastructure.Get(entity);
        }

       

        public async Task<List<InventoryInspector>> GetList(InventoryInspector entity)
        {
            return await InventoryInspectorInfrastructure.GetList(entity);
        }
        #endregion

        #region Commands
        public async Task<int> Add(InventoryInspector entity)
        {
            return await InventoryInspectorInfrastructure.Add(entity);
        }

        public async Task<bool> Update(InventoryInspector entity)
        {
            return await InventoryInspectorInfrastructure.Update(entity);
        }

        public async Task<bool> Activate(InventoryInspector entity)
        {
            return await InventoryInspectorInfrastructure.Activate(entity);
        }
        #endregion
    }
}
