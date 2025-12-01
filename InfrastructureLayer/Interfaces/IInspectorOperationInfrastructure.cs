using System.Threading.Tasks;
using System.Collections.Generic;
using AuctionPortal.Models;

namespace AuctionPortal.InfrastructureLayer.Interfaces
{
    public interface IInspectorOperationInfrastructure : IBaseInfrastructure<InspectorOperation>
    {
        Task<InspectorOperation?> Login(InspectorOperation request);
        Task<bool> Logout(InspectorOperation request);
        Task<bool> ChangePassword(InspectorOperation request);
        Task<(bool Success, string Email, string? Token, DateTime? ExpiresAt, string? FirstName)>
            ForgotPassword(InspectorOperation request);

        Task<bool> ResetPassword(InspectorOperation request);
    }
}
