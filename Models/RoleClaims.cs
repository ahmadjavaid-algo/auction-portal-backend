using System.Collections.Generic;

namespace AuctionPortal.Models
{
    public class RoleClaims
    {
        
        public int RoleId { get; set; }
        public int ClaimId { get; set; }
        public List<int> ClaimIds { get; set; } = new List<int>();

        public string? ClaimCode { get; set; }
        public string? Endpoint { get; set; }
        public string? Description { get; set; }
    }
}
