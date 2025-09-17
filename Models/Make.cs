using AuctionPortal.Common.Models;

namespace AuctionPortal.Models
{
    public class Make:BaseModel
    {
        public int MakeId { get; set; }
        public string MakeName { get; set; } = string.Empty;

    }
}

