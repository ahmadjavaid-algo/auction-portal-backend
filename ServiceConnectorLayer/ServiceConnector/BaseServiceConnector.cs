// Common/Services/BaseServiceConnector.cs
using System;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AuctionPortal.Common.Services
{
    public class BaseServiceConnector : IBaseServiceConnector
    {
        private readonly IConfiguration _config;

        public BaseServiceConnector(IConfiguration config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public string JwtKey => _config["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key missing.");
        public string JwtIssuer => _config["Jwt:Issuer"] ?? "AuctionApi";
        public string JwtAudience => _config["Jwt:Audience"] ?? "AuctionClient";
        public int JwtExpiresMinutes => int.TryParse(_config["Jwt:ExpiresMinutes"], out var m) ? m : 60;

        public SymmetricSecurityKey GetSigningKey()
        {
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtKey));
        }

        public SigningCredentials GetSigningCredentials()
        {
            return new SigningCredentials(GetSigningKey(), SecurityAlgorithms.HmacSha256);
        }

        public TokenValidationParameters GetValidationParameters()
        {
            return new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = GetSigningKey(),
                ValidateIssuer = true,
                ValidIssuer = JwtIssuer,
                ValidateAudience = true,
                ValidAudience = JwtAudience,
                ValidateLifetime = true,           
                RequireExpirationTime = true,      
                ClockSkew = TimeSpan.Zero
            };
        }
    }
}
