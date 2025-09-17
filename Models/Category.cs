using AuctionPortal.Common.Models;

namespace AuctionPortal.Models
{
    public class Category : BaseModel
    {
        public int YearId { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;

    }
}

