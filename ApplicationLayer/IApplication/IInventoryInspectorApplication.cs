using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AuctionPortal.Models;
using AuctionPortal.Common.Models;

namespace AuctionPortal.ApplicationLayer.IApplication
{
   
    public interface IInventoryInspectorApplication : IBaseApplication<InventoryInspector, int>
    {
        
    }
}
