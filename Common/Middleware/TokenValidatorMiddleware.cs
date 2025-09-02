// AuctionPortal.Common.Middleware/TokenValidatorMiddleware.cs
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using AuctionPortal.Common.Infrastructure;
using AuctionPortal.Common.Services;
using AuctionPortal.Common.Auth;   // for ClaimsConstants
using System.Security.Claims;

namespace AuctionPortal.Common.Middleware
{
    public class TokenValidatorMiddleware : BaseMiddleware
    {
        #region Constructor
        /// <summary>
        /// TokenValidatorMiddleware initializes class object.
        /// </summary>
        public TokenValidatorMiddleware(RequestDelegate next, IConfiguration configuration)
            : base(next, configuration)
        {
        }
        #endregion

        #region Constants
        private const string BodyCurrentUserIdProperty = "CurrentUserId";
        private const string ContentTypeJson = "application/json";
        #endregion

        #region Invoke
        public async Task Invoke(HttpContext context)
        {
            var authHeader = context.Request.Headers["Authorization"].ToString();

            if (!string.IsNullOrWhiteSpace(authHeader))
            {
                // Resolve ITokenService from DI (no ctor changes needed)
                var tokenService = context.RequestServices.GetRequiredService<ITokenService>();

                // Validate token and set HttpContext.User
                var principal = tokenService.ValidateToken(authHeader, out _);
                if (principal != null)
                {
                    context.User = principal;

                    // Extract user id + permissions from claims
                    var userId = GetUserIdFromClaims(principal);
                    if (userId != 0)
                    {
                        // Make available to downstream components
                        context.Items[nameof(BodyCurrentUserIdProperty)] = userId;

                        var permissions = GetPermissionsFromClaims(principal);
                        context.Items[ClaimsConstants.PermissionClaimType] = permissions;

                        // Mirror template behavior: inject CurrentUserId into query/body
                        var req = context.Request;

                        if (HttpMethods.IsGet(req.Method))
                        {
                            req.QueryString = req.QueryString.Add(BodyCurrentUserIdProperty, userId.ToString());
                        }
                        else
                        {
                            try
                            {
                                if (req.ContentType?.StartsWith(ContentTypeJson, StringComparison.OrdinalIgnoreCase) == true)
                                {
                                    req.EnableBuffering();

                                    using var reader = new StreamReader(req.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true);
                                    var original = await reader.ReadToEndAsync();
                                    req.Body.Position = 0;

                                    if (!string.IsNullOrWhiteSpace(original))
                                    {
                                        var dyn = JsonConvert.DeserializeObject<dynamic>(original) ?? new Newtonsoft.Json.Linq.JObject();
                                        dyn[BodyCurrentUserIdProperty] = userId;

                                        var rewritten = JsonConvert.SerializeObject(dyn);
                                        var bytes = Encoding.UTF8.GetBytes(rewritten);
                                        req.Body = new MemoryStream(bytes);
                                        req.ContentLength = bytes.Length;
                                    }
                                }
                            }
                            catch
                            {
                                // On any issue, keep original body and continue
                            }
                        }
                    }
                }
            }

            await this.Next(context);
        }
        #endregion

        #region Helpers
        private static int GetUserIdFromClaims(ClaimsPrincipal principal)
        {
            try
            {
                var val = principal.FindFirst(ClaimsConstants.UserIdClaimType)?.Value;
                return int.TryParse(val, out var id) ? id : 0;
            }
            catch
            {
                return 0;
            }
        }

        private static List<string> GetPermissionsFromClaims(ClaimsPrincipal principal)
        {
            try
            {
                return principal.Claims
                    .Where(c => c.Type == ClaimsConstants.PermissionClaimType && !string.IsNullOrWhiteSpace(c.Value))
                    .Select(c => c.Value.Trim())
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();
            }
            catch
            {
                return new List<string>();
            }
        }
        #endregion
    }
}
