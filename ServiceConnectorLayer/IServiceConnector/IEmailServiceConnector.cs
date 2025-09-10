// AuctionPortal.Common.Services/ITokenService.cs
using System.Collections.Generic;
using System.Security.Claims;
using AuctionPortal.Models;
using Microsoft.IdentityModel.Tokens;

namespace AuctionPortal.Common.Services
{
    public interface IEmailServiceConnector
    {
        Task<bool> SendEmail(string to, string subject, string body, string? from = null, bool isHtml = true);
    }

}
