using System.Threading.Tasks;
using System.Collections.Generic;
using AuctionPortal.Models;

namespace AuctionPortal.InfrastructureLayer.Interfaces
{
    public interface IBidderOperationInfrastructure : IBaseInfrastructure<BidderOperation>
    {
        Task<BidderOperation?> Login(BidderOperation request);
        Task<bool> Logout(BidderOperation request);
        Task<bool> ChangePassword(BidderOperation request);
        Task<(bool Success, string Email, string? Token, DateTime? ExpiresAt, string? FirstName)>
            ForgotPassword(BidderOperation request);

        Task<bool> ResetPassword(BidderOperation request);
    }
}
