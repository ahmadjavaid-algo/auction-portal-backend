// BidderApplication.cs
using AuctionPortal.ApplicationLayer.IApplication;
using AuctionPortal.InfrastructureLayer.Interfaces;
using AuctionPortal.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuctionPortal.ApplicationLayer.Application
{
    public class BidderApplication : BaseApplication, IBidderApplication
    {
        public BidderApplication(IBidderInfrastructure bidderInfrastructure, IConfiguration configuration)
            : base(configuration)
        {
            this.BidderInfrastructure = bidderInfrastructure
                ?? throw new ArgumentNullException(nameof(bidderInfrastructure));
        }

        public IBidderInfrastructure BidderInfrastructure { get; }

        #region Queries
        public async Task<Bidder> Get(Bidder entity)
        {
            return await BidderInfrastructure.Get(entity);
        }
        public async Task<Bidder> GetStats()
        {
           return await BidderInfrastructure.GetStats(); 
        }
       

        public async Task<List<Bidder>> GetList(Bidder entity)
        {
            return await BidderInfrastructure.GetList(entity);
        }
        #endregion

        #region Commands
        public async Task<int> Add(Bidder entity)
        {
            return await BidderInfrastructure.Add(entity);
        }

        public async Task<bool> Update(Bidder entity)
        {
            return await BidderInfrastructure.Update(entity);
        }

        public async Task<bool> Activate(Bidder entity)
        {
            return await BidderInfrastructure.Activate(entity);
        }
        #endregion
    }
}
