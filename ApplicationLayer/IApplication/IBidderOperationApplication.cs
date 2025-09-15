using System.Collections.Generic;
using System.Threading.Tasks;
using AuctionPortal.Models;

namespace AuctionPortal.ApplicationLayer.IApplication
{
    public interface IBidderOperationApplication : IBaseApplication<BidderOperation, int>
    {
        Task<BidderOperation> Login(BidderOperation request);
        Task<bool> Logout(BidderOperation request);
        Task<bool> ChangePassword(BidderOperation request);
        Task<bool> ForgotPassword(BidderOperation request);
        Task<bool> ResetPassword(BidderOperation request);
    }
}
