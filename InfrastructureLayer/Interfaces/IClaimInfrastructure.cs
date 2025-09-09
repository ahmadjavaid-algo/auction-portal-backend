// AuctionPortal.InfrastructureLayer.Interfaces/IClaimInfrastructure.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using AuctionPortal.Models;

namespace AuctionPortal.InfrastructureLayer.Interfaces
{
    public interface IClaimInfrastructure
    {
        Task<List<string>> GetAllClaimCodes();
        Task<List<string>> GetEffectiveClaimCodesForUser(int userId);

        // Single-DTO signatures
        Task<List<RoleClaims>> GetByRole(RoleClaims request);
        Task<bool> SetForRole(RoleClaims request);
        Task<List<RoleClaims>> GetList(RoleClaims request);

    }
}
