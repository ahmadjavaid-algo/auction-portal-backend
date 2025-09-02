using System.Threading.Tasks;
using System.Collections.Generic;
using AuctionPortal.Models;

namespace AuctionPortal.InfrastructureLayer.Interfaces
{
    public interface IApplicationUserOperationInfrastructure : IBaseInfrastructure<ApplicationUserOperation>
    {
        Task<ApplicationUserOperation?> Login(ApplicationUserOperation request);
        Task<bool> Logout(ApplicationUserOperation request);
        Task<bool> ChangePassword(ApplicationUserOperation request);
        Task<bool> ForgotPassword(ApplicationUserOperation request);
        Task<bool> ResetPassword(ApplicationUserOperation request);
    }
}
