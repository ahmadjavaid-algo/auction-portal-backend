using AuctionPortal.ApplicationLayer.IApplication;
using AuctionPortal.InfrastructureLayer.Interfaces;
using AuctionPortal.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuctionPortal.ApplicationLayer.Application
{
    public class DashboardApplication : BaseApplication, IDashboardApplication
    {
        public DashboardApplication(IDashboardInfrastructure DashboardInfrastructure, IConfiguration configuration)
            : base(configuration)
        {
            this.DashboardInfrastructure = DashboardInfrastructure ?? throw new ArgumentNullException(nameof(DashboardInfrastructure));
        }

        public IDashboardInfrastructure DashboardInfrastructure { get; }

        #region Queries
        public async Task<Dashboard> Get(Dashboard entity)
        {
            return await DashboardInfrastructure.Get(entity);
        }

        public async Task<List<Dashboard>> GetList(Dashboard entity)
        {
            return await DashboardInfrastructure.GetList(entity);
        }
        #endregion

        #region Commands
        public async Task<int> Add(Dashboard entity)
        {
            return await DashboardInfrastructure.Add(entity);
        }

        public async Task<bool> Update(Dashboard entity)
        {
            return await DashboardInfrastructure.Update(entity);
        }

        public async Task<bool> Activate(Dashboard entity)
        {
            return await DashboardInfrastructure.Activate(entity);
        }
        #endregion
    }
}
