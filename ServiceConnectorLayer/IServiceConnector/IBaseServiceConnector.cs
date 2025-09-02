// Common/Services/IBaseServiceConnector.cs
using Microsoft.IdentityModel.Tokens;

namespace AuctionPortal.Common.Services
{
    public interface IBaseServiceConnector
    {
        string JwtKey { get; }
        string JwtIssuer { get; }
        string JwtAudience { get; }
        int JwtExpiresMinutes { get; }

        SymmetricSecurityKey GetSigningKey();
        SigningCredentials GetSigningCredentials();
        TokenValidationParameters GetValidationParameters();
    }
}
