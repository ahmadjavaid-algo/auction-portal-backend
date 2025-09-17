using AuctionPortal.ApplicationLayer.IApplication;
using AuctionPortal.InfrastructureLayer.Interfaces;
using AuctionPortal.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuctionPortal.ApplicationLayer.Application
{
    public class MakeApplication : BaseApplication, IMakeApplication
    {
        public MakeApplication(IMakeInfrastructure makeInfrastructure, IConfiguration configuration)
            : base(configuration)
        {
            MakeInfrastructure = makeInfrastructure ?? throw new ArgumentNullException(nameof(makeInfrastructure));
        }

        public IMakeInfrastructure MakeInfrastructure { get; }

        #region Queries
        public async Task<Make> Get(Make entity)
        {
            return await MakeInfrastructure.Get(entity);
        }

        public async Task<List<Make>> GetList(Make entity)
        {
            return await MakeInfrastructure.GetList(entity);
        }
        #endregion

        #region Commands
        public async Task<int> Add(Make entity)
        {
            return await MakeInfrastructure.Add(entity);
        }

        public async Task<bool> Update(Make entity)
        {
            return await MakeInfrastructure.Update(entity);
        }

        public async Task<bool> Activate(Make entity)
        {
            return await MakeInfrastructure.Activate(entity);
        }
        #endregion
    }
}
