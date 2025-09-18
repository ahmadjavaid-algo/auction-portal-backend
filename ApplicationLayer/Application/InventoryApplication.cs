using AuctionPortal.ApplicationLayer.IApplication;
using AuctionPortal.InfrastructureLayer.Interfaces;
using AuctionPortal.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuctionPortal.ApplicationLayer.Application
{
    public class InventoryApplication : BaseApplication, IInventoryApplication
    {
        public InventoryApplication(IInventoryInfrastructure InventoryInfrastructure, IConfiguration configuration)
            : base(configuration)
        {
            this.InventoryInfrastructure = InventoryInfrastructure ?? throw new ArgumentNullException(nameof(InventoryInfrastructure));
        }

        public IInventoryInfrastructure InventoryInfrastructure { get; }

        #region Queries
        public async Task<Inventory> Get(Inventory entity)
        {
            return await InventoryInfrastructure.Get(entity);
        }

        public async Task<List<Inventory>> GetList(Inventory entity)
        {
            return await InventoryInfrastructure.GetList(entity);
        }
        #endregion

        #region Commands
        public async Task<int> Add(Inventory entity)
        {
            return await InventoryInfrastructure.Add(entity);
        }

        public async Task<bool> Update(Inventory entity)
        {
            return await InventoryInfrastructure.Update(entity);
        }

        public async Task<bool> Activate(Inventory entity)
        {
            return await InventoryInfrastructure.Activate(entity);
        }
        #endregion
    }
}
