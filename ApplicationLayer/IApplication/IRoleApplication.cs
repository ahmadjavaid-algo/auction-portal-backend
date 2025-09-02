using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AuctionPortal.Models;

namespace AuctionPortal.ApplicationLayer.IApplication
{
    // Inherit from the fully generic base to keep contracts consistent across modules
    public interface IRoleApplication : IBaseApplication<Role, int>
    {
        // Add role-specific methods here if you ever need any beyond the base contract.
    }
}
