// AuctionPortal.Common.Services/TokenService.cs
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using AuctionPortal.Common.Auth;
using Microsoft.IdentityModel.Tokens;

namespace AuctionPortal.Common.Services
{
    public class TokenService : ITokenService
    {
        private readonly IBaseServiceConnector _base;

        public TokenService(IBaseServiceConnector baseServiceConnector)
        {
            _base = baseServiceConnector;
        }

        /// <summary>
        /// ❗Deprecated: Always prefer the overload that accepts permissions so they are embedded in the JWT.
        /// </summary>
        [Obsolete("Use CreateToken(int userId, string userName, string? email, IEnumerable<string> permissions) so permissions are embedded in the JWT.")]
        public string CreateToken(int userId, string userName, string? email)
        {
            return CreateToken(userId, userName, email, Array.Empty<string>());
        }

        /// <summary>
        /// Creates a JWT that embeds permissions as claims. No permissions are returned in the response DTO.
        /// </summary>
        public string CreateToken(int userId, string userName, string? email, IEnumerable<string>? permissions)
        {
            var creds = _base.GetSigningCredentials();
            var expires = DateTime.UtcNow.AddMinutes(_base.JwtExpiresMinutes);

            var claims = new List<Claim>
            {
                // Must match middleware expectation (e.g., "UserId")
                new Claim(ClaimsConstants.UserIdClaimType, userId.ToString()),
                new Claim(ClaimTypes.Name, userName ?? string.Empty)
            };

            if (!string.IsNullOrWhiteSpace(email))
                claims.Add(new Claim(ClaimTypes.Email, email));

            // Embed each permission as its own claim (e.g., type = "perm")
            if (permissions != null)
            {
                foreach (var p in permissions
                                  .Where(s => !string.IsNullOrWhiteSpace(s))
                                  .Select(s => s.Trim())
                                  .Distinct(StringComparer.OrdinalIgnoreCase))
                {
                    claims.Add(new Claim(ClaimsConstants.PermissionClaimType, p));
                }
            }

            var token = new JwtSecurityToken(
                issuer: _base.JwtIssuer,
                audience: _base.JwtAudience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public ClaimsPrincipal? ValidateToken(string token, out SecurityToken? securityToken)
        {
            securityToken = null;
            if (string.IsNullOrWhiteSpace(token)) return null;

            token = token.Replace("bearer", "", StringComparison.OrdinalIgnoreCase).Replace(" ", "");

            var handler = new JwtSecurityTokenHandler();
            try
            {
                return handler.ValidateToken(token, _base.GetValidationParameters(), out securityToken);
            }
            catch
            {
                return null;
            }
        }
    }
}
