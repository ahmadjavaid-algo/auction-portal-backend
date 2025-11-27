// InspectorApplication.cs
using AuctionPortal.ApplicationLayer.IApplication;
using AuctionPortal.InfrastructureLayer.Interfaces;
using AuctionPortal.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuctionPortal.ApplicationLayer.Application
{
    public class InspectorApplication : BaseApplication, IInspectorApplication
    {
        public InspectorApplication(IInspectorInfrastructure InspectorInfrastructure, IConfiguration configuration)
            : base(configuration)
        {
            this.InspectorInfrastructure = InspectorInfrastructure
                ?? throw new ArgumentNullException(nameof(InspectorInfrastructure));
        }

        public IInspectorInfrastructure InspectorInfrastructure { get; }

        #region Queries
        public async Task<Inspector> Get(Inspector entity)
        {
            return await InspectorInfrastructure.Get(entity);
        }
        public async Task<Inspector> GetStats()
        {
           return await InspectorInfrastructure.GetStats(); 
        }
       

        public async Task<List<Inspector>> GetList(Inspector entity)
        {
            return await InspectorInfrastructure.GetList(entity);
        }
        #endregion

        #region Commands
        public async Task<int> Add(Inspector entity)
        {
            return await InspectorInfrastructure.Add(entity);
        }

        public async Task<bool> Update(Inspector entity)
        {
            return await InspectorInfrastructure.Update(entity);
        }

        public async Task<bool> Activate(Inspector entity)
        {
            return await InspectorInfrastructure.Activate(entity);
        }
        #endregion
    }
}
