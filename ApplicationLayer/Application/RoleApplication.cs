// RoleApplication.cs
using AuctionPortal.ApplicationLayer.IApplication;
using AuctionPortal.InfrastructureLayer.Interfaces;
using AuctionPortal.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuctionPortal.ApplicationLayer.Application
{
    public class RoleApplication : BaseApplication, IRoleApplication
    {
        public RoleApplication(IRoleInfrastructure roleInfrastructure, IConfiguration configuration)
            : base(configuration)
        {
            RoleInfrastructure = roleInfrastructure ?? throw new ArgumentNullException(nameof(roleInfrastructure));
        }

        public IRoleInfrastructure RoleInfrastructure { get; }

        #region Queries
        public async Task<Role> Get(Role entity)
        {
            return await RoleInfrastructure.Get(entity);
        }

        public async Task<List<Role>> GetList(Role entity)
        {
            return await RoleInfrastructure.GetList(entity);
        }
        #endregion

        #region Commands
        public async Task<int> Add(Role entity)
        {
            return await RoleInfrastructure.Add(entity);
        }

        public async Task<bool> Update(Role entity)
        {
            return await RoleInfrastructure.Update(entity);
        }

        public async Task<bool> Activate(Role entity)
        {
            return await RoleInfrastructure.Activate(entity);
        }
        #endregion
    }
}
