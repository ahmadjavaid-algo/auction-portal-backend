using System.Collections.Generic;
using AuctionPortal.Common.Models;

namespace AuctionPortal.Models
{
    public class ApplicationUserOperation : BaseModel
    {
        // Common/user info
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        // Login / password fields
        public string PasswordHash { get; set; } = string.Empty;
        public string OldPasswordHash { get; set; } = string.Empty;
        public string NewPasswordHash { get; set; } = string.Empty;
        public string ResetCode { get; set; } = string.Empty;

        // Response fields
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? Token { get; set; }
        public List<string>? Permissions { get; set; }
    }
}
