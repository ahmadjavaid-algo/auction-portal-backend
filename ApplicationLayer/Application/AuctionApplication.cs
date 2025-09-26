using AuctionPortal.ApplicationLayer.IApplication;
using AuctionPortal.InfrastructureLayer.Interfaces;
using AuctionPortal.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuctionPortal.ApplicationLayer.Application
{
    public class AuctionApplication : BaseApplication, IAuctionApplication
    {
        public AuctionApplication(IAuctionInfrastructure AuctionInfrastructure, IConfiguration configuration)
            : base(configuration)
        {
            this.AuctionInfrastructure = AuctionInfrastructure ?? throw new ArgumentNullException(nameof(AuctionInfrastructure));
        }

        public IAuctionInfrastructure AuctionInfrastructure { get; }

        #region Queries
        public async Task<Auction> Get(Auction entity)
        {
            return await AuctionInfrastructure.Get(entity);
        }

        public async Task<List<Auction>> GetList(Auction entity)
        {
            return await AuctionInfrastructure.GetList(entity);
        }
        #endregion

        #region Commands
        public async Task<int> Add(Auction entity)
        {
            return await AuctionInfrastructure.Add(entity);
        }

        public async Task<bool> Update(Auction entity)
        {
            return await AuctionInfrastructure.Update(entity);
        }

        public async Task<bool> Activate(Auction entity)
        {
            return await AuctionInfrastructure.Activate(entity);
        }
        #endregion
    }
}
