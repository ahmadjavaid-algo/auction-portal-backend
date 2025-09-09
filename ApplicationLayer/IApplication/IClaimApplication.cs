// AuctionPortal.ApplicationLayer.IApplication/IClaimApplication.cs
using AuctionPortal.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuctionPortal.ApplicationLayer.IApplication
{
    public interface IClaimApplication
    {
        Task<List<string>> GetAllClaimCodes();
        Task<List<string>> GetEffectiveClaimCodesForUser(int userId);

        // Single-DTO signatures
        Task<List<RoleClaims>> GetByRole(RoleClaims request);
        Task<bool> SetForRole(RoleClaims request);
        Task<List<RoleClaims>> GetList(RoleClaims request);


    }
}
