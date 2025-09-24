using AuctionPortal.Common.Models;

namespace AuctionPortal.Models
{
    public class Model : BaseModel
    {
        public int MakeId { get; set; }
        public string MakeName { get; set; } = string.Empty;
        public int ModelId { get; set; }
        public string ModelName { get; set; } = string.Empty;

    }
}

