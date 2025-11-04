using System.Security.Claims;
using AuctionPortal.Common.Auth;  
using Microsoft.AspNetCore.SignalR;
using AuctionPortal.Common.Infrastructure;
using AuctionPortal.InfrastructureLayer.Interfaces;
using AuctionPortal.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Data.Common;
namespace AuctionPortal.InfrastructureLayer.Infrastructure
{
    public class SignalRUserIdProvider : IUserIdProvider
    {
        public string? GetUserId(HubConnectionContext connection)
        {
            return connection.User?
                .FindFirst(ClaimsConstants.UserIdClaimType)?
                .Value;
        }
    }
}
