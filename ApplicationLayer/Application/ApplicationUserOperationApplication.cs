using AuctionPortal.ApplicationLayer.IApplication;
using AuctionPortal.Common.Services;
using AuctionPortal.InfrastructureLayer.Interfaces;
using AuctionPortal.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AuctionPortal.ApplicationLayer.Application
{
    public class ApplicationUserOperationApplication : BaseApplication, IApplicationUserOperationApplication
    {
        public IApplicationUserOperationInfrastructure ApplicationUserOperationInfrastructure { get; }
        //public IEmailServiceConnector EmailServiceConnector { get; }
        private readonly ITokenService _tokens;
        private readonly IClaimApplication _claims;

        public ApplicationUserOperationApplication(
            IApplicationUserOperationInfrastructure applicationUserOperationInfrastructure, 
            //IEmailServiceConnector emailServiceConnector,
            IClaimApplication claims,
            ITokenService tokens,
            IConfiguration configuration) : base(configuration)
        {
            ApplicationUserOperationInfrastructure = applicationUserOperationInfrastructure
                ?? throw new ArgumentNullException(nameof(applicationUserOperationInfrastructure));
            _claims = claims ?? throw new ArgumentNullException(nameof(claims));
            _tokens = tokens ?? throw new ArgumentNullException(nameof(tokens));
            //this.EmailServiceConnector = emailServiceConnector;
        }

        #region Queries
        public Task<ApplicationUserOperation> Get(ApplicationUserOperation entity)
        {
            return ApplicationUserOperationInfrastructure.Get(entity);
        }

        public Task<System.Collections.Generic.List<ApplicationUserOperation>> GetList(ApplicationUserOperation entity)
        {
            return ApplicationUserOperationInfrastructure.GetList(entity);
        }
        #endregion

        #region Commands
        public Task<int> Add(ApplicationUserOperation entity)
        {
            return ApplicationUserOperationInfrastructure.Add(entity);
        }

        public Task<bool> Update(ApplicationUserOperation entity)
        {
            return ApplicationUserOperationInfrastructure.Update(entity);
        }

        public Task<bool> Activate(ApplicationUserOperation entity)
        {
            return ApplicationUserOperationInfrastructure.Activate(entity);
        }

        public async Task<ApplicationUserOperation> Login(ApplicationUserOperation request)
        {
            var resp = await ApplicationUserOperationInfrastructure.Login(request);

            if (resp?.Success == true)
            {
                
                var perms = await _claims.GetEffectiveClaimCodesForUser(resp.UserId);
                resp.Token = _tokens.CreateToken(resp.UserId, resp.UserName ?? string.Empty, resp.Email, perms);
               
                return resp;
            }

            return new ApplicationUserOperation { Success = false, Message = resp?.Message ?? "Invalid credentials" };
        }

        public Task<bool> Logout(ApplicationUserOperation request)
        {
            return ApplicationUserOperationInfrastructure.Logout(request);
        }

        public Task<bool> ChangePassword(ApplicationUserOperation request)
        {
            return ApplicationUserOperationInfrastructure.ChangePassword(request);
        }

        public Task<bool> ForgotPassword(ApplicationUserOperation request)
        {
            //Email email = EmailInfrastructure.Get("CODE_FRGT")

            
            // EmailServiceConnector.SendEmail(email);
            return ApplicationUserOperationInfrastructure.ForgotPassword(request);
        }

        public Task<bool> ResetPassword(ApplicationUserOperation request)
        {
            return ApplicationUserOperationInfrastructure.ResetPassword(request);
        }
        #endregion
    }
}
