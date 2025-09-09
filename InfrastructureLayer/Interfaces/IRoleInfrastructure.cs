using System;
using AuctionPortal.Models;

namespace AuctionPortal.InfrastructureLayer.Interfaces
{
    public interface IRoleInfrastructure : IBaseInfrastructure<Role>
    {
        Task<Role> GetStats();
    }
}
