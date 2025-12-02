using AuctionPortal.ApplicationLayer.IApplication;
using AuctionPortal.InfrastructureLayer.Interfaces;
using AuctionPortal.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuctionPortal.ApplicationLayer.Application
{
    public class InspectionApplication : BaseApplication, IInspectionApplication
    {
        public InspectionApplication(IInspectionInfrastructure InspectionInfrastructure, IConfiguration configuration)
            : base(configuration)
        {
            this.InspectionInfrastructure = InspectionInfrastructure
                ?? throw new ArgumentNullException(nameof(InspectionInfrastructure));
        }

        public IInspectionInfrastructure InspectionInfrastructure { get; }

        #region Queries
        public async Task<Inspection> Get(Inspection entity)
        {
            return await InspectionInfrastructure.Get(entity);
        }

        public async Task<List<Inspection>> GetList(Inspection entity)
        {
            return await InspectionInfrastructure.GetList(entity);
        }

        public async Task<List<Inspection>> GetByInventory(Inspection entity)
        {
            return await InspectionInfrastructure.GetByInventory(entity);
        }
        #endregion

        #region Commands
        public async Task<int> Add(Inspection entity)
        {
            return await InspectionInfrastructure.Add(entity);
        }

        public async Task<bool> Update(Inspection entity)
        {
            return await InspectionInfrastructure.Update(entity);
        }

        public async Task<bool> Activate(Inspection entity)
        {
            return await InspectionInfrastructure.Activate(entity);
        }
        #endregion
    }
}
