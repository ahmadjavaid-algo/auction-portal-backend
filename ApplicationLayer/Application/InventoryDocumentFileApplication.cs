using AuctionPortal.ApplicationLayer.IApplication;
using AuctionPortal.InfrastructureLayer.Interfaces;
using AuctionPortal.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuctionPortal.ApplicationLayer.Application
{
    public class InventoryDocumentFileApplication : BaseApplication, IInventoryDocumentFileApplication
    {
        public InventoryDocumentFileApplication(IInventoryDocumentFileInfrastructure InventoryDocumentFileInfrastructure, IConfiguration configuration)
            : base(configuration)
        {
            this.InventoryDocumentFileInfrastructure = InventoryDocumentFileInfrastructure ?? throw new ArgumentNullException(nameof(InventoryDocumentFileInfrastructure));
        }

        public IInventoryDocumentFileInfrastructure InventoryDocumentFileInfrastructure { get; }

        #region Queries
        public async Task<InventoryDocumentFile> Get(InventoryDocumentFile entity)
        {
            return await InventoryDocumentFileInfrastructure.Get(entity);
        }

        public async Task<List<InventoryDocumentFile>> GetList(InventoryDocumentFile entity)
        {
            return await InventoryDocumentFileInfrastructure.GetList(entity);
        }
        #endregion

        #region Commands
        public async Task<int> Add(InventoryDocumentFile entity)
        {
            return await InventoryDocumentFileInfrastructure.Add(entity);
        }
  

        public async Task<bool> Update(InventoryDocumentFile entity)
        {
            return await InventoryDocumentFileInfrastructure.Update(entity);
        }

        public async Task<bool> Activate(InventoryDocumentFile entity)
        {
            return await InventoryDocumentFileInfrastructure.Activate(entity);
        }
        #endregion
    }
}
