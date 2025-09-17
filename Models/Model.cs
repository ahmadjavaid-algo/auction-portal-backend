using AuctionPortal.Common.Models;

namespace AuctionPortal.Models
{
    public class Model : BaseModel
    {
        public int MakeId { get; set; }
        public int ModelId { get; set; }
        public string ModelName { get; set; } = string.Empty;

    }
}

