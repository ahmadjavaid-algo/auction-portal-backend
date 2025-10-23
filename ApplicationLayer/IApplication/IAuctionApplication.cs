using AuctionPortal.Models;

namespace AuctionPortal.ApplicationLayer.IApplication
{
    public interface IAuctionApplication : IBaseApplication<Auction, int>
    {
        Task<AuctionTimebox> GetTimebox(Auction entity);
    }
}
