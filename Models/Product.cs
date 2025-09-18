using AuctionPortal.Common.Models;

namespace AuctionPortal.Models
{
    public class Product : BaseModel
    {
        public int MakeId { get; set; }
        public int ModelId { get; set; }
        public int YearId { get; set; }
        public int CategoryId { get; set; }
        public int ProductId { get; set; }
        public string DisplayName { get; set; } = string.Empty;
    }
}
