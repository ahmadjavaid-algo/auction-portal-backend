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
    public class BidderOperationApplication : BaseApplication, IBidderOperationApplication
    {
        public IBidderOperationInfrastructure BidderOperationInfrastructure { get; }
        
        private readonly ITokenService _tokens;
        private readonly IClaimApplication _claims;
        private readonly IEmailServiceConnector _email;
        private readonly IEmailsInfrastructure _emails;  
        public BidderOperationApplication(
            IBidderOperationInfrastructure BidderOperationInfrastructure,
            IClaimApplication claims,
            ITokenService tokens,
            IEmailServiceConnector email,
            IEmailsInfrastructure emails,
            IConfiguration configuration) : base(configuration)
        {
            this.BidderOperationInfrastructure = BidderOperationInfrastructure
                ?? throw new ArgumentNullException(nameof(BidderOperationInfrastructure));
            _claims = claims ?? throw new ArgumentNullException(nameof(claims));
            _tokens = tokens ?? throw new ArgumentNullException(nameof(tokens));
            _email = email ?? throw new ArgumentNullException(nameof(email));
            _emails = emails ?? throw new ArgumentNullException(nameof(emails));
        }

        #region Queries
        public Task<BidderOperation> Get(BidderOperation entity)
        {
            return BidderOperationInfrastructure.Get(entity);
        }

        public Task<System.Collections.Generic.List<BidderOperation>> GetList(BidderOperation entity)
        {
            return BidderOperationInfrastructure.GetList(entity);
        }
        #endregion

        #region Commands
        public Task<int> Add(BidderOperation entity)
        {
            return BidderOperationInfrastructure.Add(entity);
        }

        public Task<bool> Update(BidderOperation entity)
        {
            return BidderOperationInfrastructure.Update(entity);
        }

        public Task<bool> Activate(BidderOperation entity)
        {
            return BidderOperationInfrastructure.Activate(entity);
        }

        public async Task<BidderOperation> Login(BidderOperation request)
        {
            var resp = await BidderOperationInfrastructure.Login(request);

            if (resp?.Success == true)
            {
                
                var perms = await _claims.GetEffectiveClaimCodesForUser(resp.UserId);
                resp.Token = _tokens.CreateToken(resp.UserId, resp.UserName ?? string.Empty, resp.Email, perms);
               
                return resp;
            }

            return new BidderOperation { Success = false, Message = resp?.Message ?? "Invalid credentials" };
        }

        public Task<bool> Logout(BidderOperation request)
        {
            return BidderOperationInfrastructure.Logout(request);
        }

        public Task<bool> ChangePassword(BidderOperation request)
        {
            return BidderOperationInfrastructure.ChangePassword(request);
        }

        public async Task<bool> ForgotPassword(BidderOperation request)
        {
            var res = await BidderOperationInfrastructure.ForgotPassword(request);

            if (!string.IsNullOrWhiteSpace(res.Token))
            {
                var uiBase = Configuration["Clientbidder:BaseUrl"] ?? "http://localhost:4200";
                var resetPath = Configuration["Clientbidder:ResetPath"] ?? "/bidder/auth/reset-password";

                
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


        public Task<bool> ResetPassword(BidderOperation request)
        {
            return BidderOperationInfrastructure.ResetPassword(request);
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
