using System;
using AuctionPortal.Common.Models;

namespace AuctionPortal.Models
{
    public class AdminNotification : BaseModel
    {
        public int AdminNotificationId { get; set; }

        public string Type { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;

        public bool IsRead { get; set; }
        public DateTime? ReadDate { get; set; }

        public int? AffectedUserId { get; set; }
        public int? AuctionId { get; set; }
        public int? InventoryAuctionId { get; set; }
    }
}
