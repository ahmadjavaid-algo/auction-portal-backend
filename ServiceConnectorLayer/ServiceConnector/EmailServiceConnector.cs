using System.Net;
using System.Net.Mail;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace AuctionPortal.Common.Services
{
    public class EmailServiceConnector : IEmailServiceConnector
    {
        private readonly IConfiguration _cfg;
        public EmailServiceConnector(IConfiguration cfg) => _cfg = cfg;

        public async Task<bool> SendEmail(string to, string subject, string body, string? from = null, bool isHtml = true)
        {
            var host = _cfg["Smtp:Host"];
            var user = _cfg["Smtp:User"];
            var pass = _cfg["Smtp:Pass"];
            var port = int.TryParse(_cfg["Smtp:Port"], out var p) ? p : 587;
            var enableSsl = !string.Equals(_cfg["Smtp:EnableSsl"], "false", StringComparison.OrdinalIgnoreCase);

            
            var configuredFrom = _cfg["Smtp:From"];
            var fromAddr = from ?? configuredFrom ?? user ?? "no-reply@auctionportal.local";
            if (!string.IsNullOrWhiteSpace(host) && host.Contains("smtp.gmail.com", StringComparison.OrdinalIgnoreCase))
            {
                
                if (!string.Equals(fromAddr, user, StringComparison.OrdinalIgnoreCase))
                    fromAddr = user ?? fromAddr;
            }

            
            if (string.IsNullOrWhiteSpace(host))
                return true;

            using var client = new SmtpClient(host, port)
            {
                EnableSsl = enableSsl,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = string.IsNullOrWhiteSpace(user)
                    ? CredentialCache.DefaultNetworkCredentials
                    : new NetworkCredential(user, pass),
                Timeout = 20000 // 20s
            };

            using var msg = new MailMessage
            {
                From = new MailAddress(fromAddr),
                Subject = subject ?? string.Empty,
                Body = body ?? string.Empty,
                IsBodyHtml = isHtml,
                BodyEncoding = Encoding.UTF8,
                SubjectEncoding = Encoding.UTF8
            };
            msg.To.Add(to);

            await client.SendMailAsync(msg);
            return true;
        }
    }
}
