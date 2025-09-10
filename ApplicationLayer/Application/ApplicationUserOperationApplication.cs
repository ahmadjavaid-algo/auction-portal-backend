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
        
        private readonly ITokenService _tokens;
        private readonly IClaimApplication _claims;
        private readonly IEmailServiceConnector _email;

        public ApplicationUserOperationApplication(
            IApplicationUserOperationInfrastructure applicationUserOperationInfrastructure,
            IClaimApplication claims,
            ITokenService tokens,
            IEmailServiceConnector email,              
            IConfiguration configuration) : base(configuration)
        {
            ApplicationUserOperationInfrastructure = applicationUserOperationInfrastructure
                ?? throw new ArgumentNullException(nameof(applicationUserOperationInfrastructure));
            _claims = claims ?? throw new ArgumentNullException(nameof(claims));
            _tokens = tokens ?? throw new ArgumentNullException(nameof(tokens));
            _email = email ?? throw new ArgumentNullException(nameof(email));
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

        public async Task<bool> ForgotPassword(ApplicationUserOperation request)
        {
            var res = await ApplicationUserOperationInfrastructure.ForgotPassword(request);
            
            if (!string.IsNullOrWhiteSpace(res.Token))
            {
                var uiBase = Configuration["Client:BaseUrl"] ?? "http://localhost:4200";
                var link = $"{uiBase.TrimEnd('/')}/auth/reset-password" +
                           $"?email={Uri.EscapeDataString(res.Email)}" +
                           $"&code={Uri.EscapeDataString(res.Token)}";

                var first = string.IsNullOrWhiteSpace(res.FirstName) ? "there" : res.FirstName;
                var subject = "Reset your password";
                var body = $@"
                <p>Hi {first},</p>
                <p>Click the link below to reset your password (valid until {res.ExpiresAt:yyyy-MM-dd HH:mm} UTC):</p>
                <p><a href=""{link}"">{link}</a></p>
                <p>If you didn’t request this, you can ignore this email.</p>
                <p>— Auction Portal</p>";

                
                try { await _email.SendEmail(res.Email, subject, body); } catch { }
            }

            return true;
        }

        public Task<bool> ResetPassword(ApplicationUserOperation request)
        {
            return ApplicationUserOperationInfrastructure.ResetPassword(request);
        }
        #endregion
    }
}
