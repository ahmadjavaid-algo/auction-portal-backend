using AuctionPortal.Common.Models;

namespace AuctionPortal.Models
{
    public class Role : BaseModel
    {
        #region Columns
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string RoleCode { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int TotalRoles { get; set; }
        public int ActiveRoles { get; set; }
        public int InactiveRoles { get; set; }
        #endregion
    }

}
