using AuctionPortal.ApplicationLayer.IApplication;
using AuctionPortal.InfrastructureLayer.Interfaces;
using AuctionPortal.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuctionPortal.ApplicationLayer.Application
{
    public class FavouriteApplication : BaseApplication, IFavouriteApplication
    {
        public FavouriteApplication(IFavouriteInfrastructure FavouriteInfrastructure, IConfiguration configuration)
            : base(configuration)
        {
            this.FavouriteInfrastructure = FavouriteInfrastructure ?? throw new ArgumentNullException(nameof(FavouriteInfrastructure));
        }

        public IFavouriteInfrastructure FavouriteInfrastructure { get; }

        #region Queries
        public async Task<Favourite> Get(Favourite entity)
        {
            return await FavouriteInfrastructure.Get(entity);
        }

        public async Task<List<Favourite>> GetList(Favourite entity)
        {
            return await FavouriteInfrastructure.GetList(entity);
        }
        #endregion

        #region Commands
        public async Task<int> Add(Favourite entity)
        {
            return await FavouriteInfrastructure.Add(entity);
        }

        public async Task<bool> Update(Favourite entity)
        {
            return await FavouriteInfrastructure.Update(entity);
        }

        public async Task<bool> Activate(Favourite entity)
        {
            return await FavouriteInfrastructure.Activate(entity);
        }
        #endregion
    }
}
