using AuctionPortal.ApplicationLayer.IApplication;
using AuctionPortal.InfrastructureLayer.Interfaces;
using AuctionPortal.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuctionPortal.ApplicationLayer.Application
{
    public class InventoryAuctionApplication : BaseApplication, IInventoryAuctionApplication
    {
        public InventoryAuctionApplication(IInventoryAuctionInfrastructure InventoryAuctionInfrastructure, IConfiguration configuration)
            : base(configuration)
        {
            this.InventoryAuctionInfrastructure = InventoryAuctionInfrastructure ?? throw new ArgumentNullException(nameof(InventoryAuctionInfrastructure));
        }

        public IInventoryAuctionInfrastructure InventoryAuctionInfrastructure { get; }

        #region Queries
        public async Task<InventoryAuction> Get(InventoryAuction entity)
        {
            return await InventoryAuctionInfrastructure.Get(entity);
        }

        public async Task<List<InventoryAuction>> GetList(InventoryAuction entity)
        {
            return await InventoryAuctionInfrastructure.GetList(entity);
        }
        #endregion

        #region Commands
        public async Task<int> Add(InventoryAuction entity)
        {
            return await InventoryAuctionInfrastructure.Add(entity);
        }

        public async Task<bool> Update(InventoryAuction entity)
        {
            return await InventoryAuctionInfrastructure.Update(entity);
        }

        public async Task<bool> Activate(InventoryAuction entity)
        {
            return await InventoryAuctionInfrastructure.Activate(entity);
        }
        #endregion
    }
}
