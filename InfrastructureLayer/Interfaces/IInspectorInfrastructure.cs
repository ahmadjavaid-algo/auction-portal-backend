using System;
using AuctionPortal.Models;

namespace AuctionPortal.InfrastructureLayer.Interfaces
{
    public interface IInspectorInfrastructure : IBaseInfrastructure<Inspector>
    {
        Task<Inspector> GetStats();
    }
}
