using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AuctionPortal.Models;
using AuctionPortal.Common.Models; // CreateUserRequest, UpdateUserRequest

namespace AuctionPortal.ApplicationLayer.IApplication
{
    // Leverage the fully generic base contract
    public interface IBidderApplication : IBaseApplication<Bidder, int>
    {
        Task<Bidder> GetStats();
    }
}
