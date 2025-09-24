using AuctionPortal.Common.Models;

namespace AuctionPortal.Models
{
    public class Product : BaseModel
    {
        public int MakeId { get; set; }
        public string MakeName { get; set; } = string.Empty;
        public int ModelId { get; set; }
        public string ModelName { get; set; } = string.Empty;
        public int YearId { get; set; }
        public string YearName { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int ProductId { get; set; }
        public string DisplayName { get; set; } = string.Empty;
    }
}
