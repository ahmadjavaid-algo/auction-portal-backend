// AuctionPortal.ApplicationLayer.Application/ClaimApplication.cs
using AuctionPortal.ApplicationLayer.IApplication;
using AuctionPortal.InfrastructureLayer.Interfaces;
using AuctionPortal.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuctionPortal.ApplicationLayer.Application
{
    public class ClaimApplication : BaseApplication, IClaimApplication
    {
        public ClaimApplication(IClaimInfrastructure claimInfrastructure, IConfiguration configuration)
            : base(configuration)
        {
            ClaimInfrastructure = claimInfrastructure ?? throw new ArgumentNullException(nameof(claimInfrastructure));
        }

        public IClaimInfrastructure ClaimInfrastructure { get; }

        public async Task<List<string>> GetAllClaimCodes()
        {
            return await ClaimInfrastructure.GetAllClaimCodes();
        }

        public async Task<List<string>> GetEffectiveClaimCodesForUser(int userId)
        {
            return await ClaimInfrastructure.GetEffectiveClaimCodesForUser(userId);
        }

        public Task<List<RoleClaims>> GetByRole(RoleClaims request)
        {
            return ClaimInfrastructure.GetByRole(request);
        }

        public Task<bool> SetForRole(RoleClaims request)
        {
            return ClaimInfrastructure.SetForRole(request);
        }
        public Task<List<RoleClaims>> GetList(RoleClaims request)
        {
            return ClaimInfrastructure.GetList(request);
        }



    }
}
