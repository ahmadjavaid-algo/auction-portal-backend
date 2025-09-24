using AuctionPortal.Common.Models;

namespace AuctionPortal.Models
{
    public class Category : BaseModel
    {
        public int YearId { get; set; }
        public string YearName { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;

    }
}

