using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AuctionPortal.Models;
using AuctionPortal.Common.Models;

namespace AuctionPortal.ApplicationLayer.IApplication
{
    public interface IInspectionApplication : IBaseApplication<Inspection, int>
    {
        Task<List<Inspection>> GetByInventory(Inspection entity);
    }
}
