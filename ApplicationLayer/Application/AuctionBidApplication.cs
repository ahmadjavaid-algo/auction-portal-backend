using AuctionPortal.ApplicationLayer.IApplication;
using AuctionPortal.InfrastructureLayer.Interfaces;
using AuctionPortal.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuctionPortal.ApplicationLayer.Application
{
    public class AuctionBidApplication : BaseApplication, IAuctionBidApplication
    {
        public AuctionBidApplication(IAuctionBidInfrastructure AuctionBidInfrastructure, IConfiguration configuration)
            : base(configuration)
        {
            this.AuctionBidInfrastructure = AuctionBidInfrastructure ?? throw new ArgumentNullException(nameof(AuctionBidInfrastructure));
        }

        public IAuctionBidInfrastructure AuctionBidInfrastructure { get; }

        #region Queries
        public async Task<AuctionBid> Get(AuctionBid entity)
        {
            return await AuctionBidInfrastructure.Get(entity);
        }

        public async Task<List<AuctionBid>> GetList(AuctionBid entity)
        {
            return await AuctionBidInfrastructure.GetList(entity);
        }
        #endregion

        #region Commands
        public async Task<int> Add(AuctionBid entity)
        {
            return await AuctionBidInfrastructure.Add(entity);
        }

        public async Task<bool> Update(AuctionBid entity)
        {
            return await AuctionBidInfrastructure.Update(entity);
        }

        public async Task<bool> Activate(AuctionBid entity)
        {
            return await AuctionBidInfrastructure.Activate(entity);
        }
        #endregion
    }
}
