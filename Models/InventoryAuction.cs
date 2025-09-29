using System;
using AuctionPortal.Common.Models;

namespace AuctionPortal.Models
{
    public class InventoryAuction : BaseModel
    {
        public int InventoryAuctionId { get; set; }
        public int InventoryId { get; set; }
        public int AuctionId { get; set; }

        public int InventoryAuctionStatusId { get; set; }
        

        public int BidIncrement { get; set; }
        public int BuyNowPrice { get; set; }
        public int ReservePrice { get; set; }

        public string? InventoryAuctionStatusCode { get; set; }
        public string? InventoryAuctionStatusName { get; set; }
    }
}
