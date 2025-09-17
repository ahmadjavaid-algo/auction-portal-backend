using AuctionPortal.ApplicationLayer.IApplication;
using AuctionPortal.InfrastructureLayer.Interfaces;
using AuctionPortal.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuctionPortal.ApplicationLayer.Application
{
    public class YearApplication : BaseApplication, IYearApplication
    {
        public YearApplication(IYearInfrastructure YearInfrastructure, IConfiguration configuration)
            : base(configuration)
        {
            this.YearInfrastructure = YearInfrastructure ?? throw new ArgumentNullException(nameof(YearInfrastructure));
        }

        public IYearInfrastructure YearInfrastructure { get; }

        #region Queries
        public async Task<Year> Get(Year entity)
        {
            return await YearInfrastructure.Get(entity);
        }

        public async Task<List<Year>> GetList(Year entity)
        {
            return await YearInfrastructure.GetList(entity);
        }
        #endregion

        #region Commands
        public async Task<int> Add(Year entity)
        {
            return await YearInfrastructure.Add(entity);
        }

        public async Task<bool> Update(Year entity)
        {
            return await YearInfrastructure.Update(entity);
        }

        public async Task<bool> Activate(Year entity)
        {
            return await YearInfrastructure.Activate(entity);
        }
        #endregion
    }
}
