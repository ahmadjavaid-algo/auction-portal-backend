using AuctionPortal.Common.Models;

namespace AuctionPortal.Models
{
    public class Year : BaseModel
    {
        public int YearId { get; set; }
        public int ModelId { get; set; }
        public string YearName { get; set; } = string.Empty;

    }
}

