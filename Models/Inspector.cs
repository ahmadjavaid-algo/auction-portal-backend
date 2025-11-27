using AuctionPortal.Common.Models;

namespace AuctionPortal.Models
{
    public class Inspector : BaseModel
    {
        #region Columns
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string? LastName { get; set; }
        public string? IdentificationNumber { get; set; }
        public string? Address1 { get; set; }
        public string? PostalCode { get; set; }
        public string Email { get; set; } = string.Empty;
        public bool EmailConfirmed { get; set; }
        public string? PasswordHash { get; set; }
        public string? SecurityStamp { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? LoginDate { get; set; }
        public string? Code { get; set; }
        public List<int>? RoleId { get; set; }
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int InactiveUsers { get; set; }
        #endregion
    }
}
