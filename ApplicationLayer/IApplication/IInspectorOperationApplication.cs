using System.Collections.Generic;
using System.Threading.Tasks;
using AuctionPortal.Models;

namespace AuctionPortal.ApplicationLayer.IApplication
{
    public interface IInspectorOperationApplication : IBaseApplication<InspectorOperation, int>
    {
        Task<InspectorOperation> Login(InspectorOperation request);
        Task<bool> Logout(InspectorOperation request);
        Task<bool> ChangePassword(InspectorOperation request);
        Task<bool> ForgotPassword(InspectorOperation request);
        Task<bool> ResetPassword(InspectorOperation request);
    }
}
