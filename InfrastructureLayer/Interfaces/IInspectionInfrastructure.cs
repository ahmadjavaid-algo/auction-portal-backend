using System.Collections.Generic;
using System.Threading.Tasks;
using AuctionPortal.Models;

namespace AuctionPortal.InfrastructureLayer.Interfaces
{
    public interface IInspectionInfrastructure : IBaseInfrastructure<Inspection>
    {
        Task<List<Inspection>> GetByInventory(Inspection entity);
    }
}
