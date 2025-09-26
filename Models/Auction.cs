using System;
using AuctionPortal.Common.Models;

namespace AuctionPortal.Models
{
    public class Auction : BaseModel
    {
        public int AuctionId { get; set; }

        public int AuctionStatusId { get; set; }
        public string AuctionName { get; set; } = string.Empty;

        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }

        public int BidIncrement { get; set; }

        public string? AuctionStatusCode { get; set; }
        public string? AuctionStatusName { get; set; }
    }
}
