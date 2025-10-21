// AuctionPortal.InfrastructureLayer.Interfaces/IMakeInfrastructure.cs
using AuctionPortal.Models;

namespace AuctionPortal.InfrastructureLayer.Interfaces
{
    public interface IAuctionInfrastructure : IBaseInfrastructure<Auction>
    {
        Task<int> RecalculateStatuses();
    }
}
