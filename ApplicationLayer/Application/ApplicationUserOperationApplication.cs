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
        private readonly IEmailsInfrastructure _emails;  
        public ApplicationUserOperationApplication(
            IApplicationUserOperationInfrastructure applicationUserOperationInfrastructure,
            IClaimApplication claims,
            ITokenService tokens,
            IEmailServiceConnector email,
            IEmailsInfrastructure emails,
            IConfiguration configuration) : base(configuration)
        {
            ApplicationUserOperationInfrastructure = applicationUserOperationInfrastructure
                ?? throw new ArgumentNullException(nameof(applicationUserOperationInfrastructure));
            _claims = claims ?? throw new ArgumentNullException(nameof(claims));
            _tokens = tokens ?? throw new ArgumentNullException(nameof(tokens));
            _email = email ?? throw new ArgumentNullException(nameof(email));
            _emails = emails ?? throw new ArgumentNullException(nameof(emails));
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
                var resetPath = Configuration["Client:ResetPath"] ?? "/admin/auth/reset-password";

                // Ensure exactly one slash between base and path
                var basePart = uiBase.TrimEnd('/');
                var pathPart = resetPath.StartsWith("/") ? resetPath : "/" + resetPath;

                var qs = $"?email={Uri.EscapeDataString(res.Email)}&code={Uri.EscapeDataString(res.Token)}";
                var link = $"{basePart}{pathPart}{qs}";

                var first = string.IsNullOrWhiteSpace(res.FirstName) ? "there" : res.FirstName;

                var tmpl = await _emails.GetByCode("USER_RESET_PASSWORD");
                var subject = tmpl?.EmailSubject ?? "Reset your password";
                var from = tmpl?.EmailFrom;
                var body = tmpl?.EmailBody
                             ?? $@"<p>Hi {first},</p>
                           <p>Click the link to reset your password (valid until {res.ExpiresAt:yyyy-MM-dd HH:mm} UTC):</p>
                           <p><a href=""{link}"">{link}</a></p>
                           <p>— Auction Portal</p>";

                body = Merge(body, first, res.Email, link, res.ExpiresAt);
                subject = Merge(subject, first, res.Email, link, res.ExpiresAt);

                try { await _email.SendEmail(res.Email, subject, body, from, isHtml: true); } catch { /* log if you want */ }
            }

            return true;
        }


        public Task<bool> ResetPassword(ApplicationUserOperation request)
        {
            return ApplicationUserOperationInfrastructure.ResetPassword(request);
        }
        private static string Merge(string template, string firstName, string email, string resetLink, DateTime? expiresUtc)
        {
            if (string.IsNullOrEmpty(template)) return template ?? string.Empty;
            return template
                .Replace("{{FirstName}}", firstName ?? string.Empty)
                .Replace("{{Email}}", email ?? string.Empty)
                .Replace("{{ResetLink}}", resetLink ?? string.Empty)
                .Replace("{{ExpiresAt}}", expiresUtc?.ToString("yyyy-MM-dd HH:mm 'UTC'") ?? string.Empty);
        }
        #endregion
    }
}
