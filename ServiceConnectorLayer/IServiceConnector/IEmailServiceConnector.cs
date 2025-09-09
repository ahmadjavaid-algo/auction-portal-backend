// AuctionPortal.Common.Services/ITokenService.cs
using System.Collections.Generic;
using System.Security.Claims;
using AuctionPortal.Models;
using Microsoft.IdentityModel.Tokens;

namespace AuctionPortal.Common.Services
{
    public interface IEmailServiceConnector
    {
        /// <summary>
        /// ❗Deprecated: prefer the overload that takes permissions.
        /// </summary>
       public Task<bool> SendEmail(EmailServiceConnector email);

        
    }
}
