using System;
using AuctionPortal.Models;

namespace AuctionPortal.InfrastructureLayer.Interfaces
{
    public interface IBidderInfrastructure : IBaseInfrastructure<Bidder>
    {
        Task<Bidder> GetStats();
    }
}
