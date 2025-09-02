// AuctionPortal.Common.Services/ITokenService.cs
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace AuctionPortal.Common.Services
{
    public interface ITokenService
    {
        /// <summary>
        /// ❗Deprecated: prefer the overload that takes permissions.
        /// </summary>
        string CreateToken(int userId, string userName, string? email);

        /// <summary>
        /// Creates a JWT that embeds the provided permissions as claims.
        /// </summary>
        string CreateToken(int userId, string userName, string? email, IEnumerable<string>? permissions);

        ClaimsPrincipal? ValidateToken(string token, out SecurityToken? securityToken);
    }
}
