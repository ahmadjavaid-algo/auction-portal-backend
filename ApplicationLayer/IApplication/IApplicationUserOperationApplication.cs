using System.Collections.Generic;
using System.Threading.Tasks;
using AuctionPortal.Models;

namespace AuctionPortal.ApplicationLayer.IApplication
{
    public interface IApplicationUserOperationApplication : IBaseApplication<ApplicationUserOperation, int>
    {
        Task<ApplicationUserOperation> Login(ApplicationUserOperation request);
        Task<bool> Logout(ApplicationUserOperation request);
        Task<bool> ChangePassword(ApplicationUserOperation request);
        Task<bool> ForgotPassword(ApplicationUserOperation request);
        Task<bool> ResetPassword(ApplicationUserOperation request);
    }
}
