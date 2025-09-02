// UserApplication.cs
using AuctionPortal.ApplicationLayer.IApplication;
using AuctionPortal.InfrastructureLayer.Interfaces;
using AuctionPortal.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuctionPortal.ApplicationLayer.Application
{
    public class UserApplication : BaseApplication, IUserApplication
    {
        public UserApplication(IUserInfrastructure userInfrastructure, IConfiguration configuration)
            : base(configuration)
        {
            UserInfrastructure = userInfrastructure ?? throw new ArgumentNullException(nameof(userInfrastructure));
        }

        public IUserInfrastructure UserInfrastructure { get; }

        #region Queries
        public async Task<User> Get(User entity)
        {
            return await UserInfrastructure.Get(entity);
        }

        public async Task<List<User>> GetList(User entity)
        {
            return await UserInfrastructure.GetList(entity);
        }
        #endregion

        #region Commands
        public async Task<int> Add(User entity)
        {
            return await UserInfrastructure.Add(entity);
        }

        public async Task<bool> Update(User entity)
        {
            return await UserInfrastructure.Update(entity);
        }

        public async Task<bool> Activate(User entity)
        {
            return await UserInfrastructure.Activate(entity);
        }
        #endregion
    }
}
