using AuctionPortal.Common.Models;

namespace AuctionPortal.Models
{

    public class AuctionTimebox : BaseModel
    {
        public int AuctionId { get; set; }
        public long StartEpochMsUtc { get; set; }
        public long EndEpochMsUtc { get; set; }
        public long NowEpochMsUtc { get; set; }

        public int AuctionStatusId { get; set; }
        public string AuctionStatusCode { get; set; } = string.Empty;
        public string AuctionStatusName { get; set; } = string.Empty;
    }
}
