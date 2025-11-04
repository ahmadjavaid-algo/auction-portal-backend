namespace AuctionPortal.Models
{
    public class FavouriteNotification
    {
        public int FavouriteId { get; set; }
        public int UserId { get; set; }
        public int InventoryAuctionId { get; set; }
        public int AuctionId { get; set; }

        public string Title { get; set; } = string.Empty;
        public long? StartEpochMsUtc { get; set; }
        public long? EndEpochMsUtc { get; set; }
    }
}
