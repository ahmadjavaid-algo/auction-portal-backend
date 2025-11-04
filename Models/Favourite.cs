using System;
using AuctionPortal.Common.Models;

namespace AuctionPortal.Models
{
    public class Favourite : BaseModel
    {
        public int BidderInventoryAuctionFavoriteId { get; set; }

        public int UserId { get; set; }

        public int InventoryAuctionId { get; set; }
    }
}
