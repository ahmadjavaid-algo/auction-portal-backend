using System;
using AuctionPortal.Common.Models;

namespace AuctionPortal.Models
{
    public class AuctionBid : BaseModel
    {
        public int AuctionBidId { get; set; }

        public int AuctionId { get; set; }

        public int AuctionBidStatusId { get; set; }

        public int InventoryAuctionId { get; set; }

        public decimal BidAmount { get; set; }

        public string? AuctionBidStatusName { get; set; }
    }
}
