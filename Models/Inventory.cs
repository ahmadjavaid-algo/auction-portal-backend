using AuctionPortal.Common.Models;

namespace AuctionPortal.Models
{
    public class Inventory:BaseModel
    {
        public int InventoryId { get; set; }
        public int ProductId { get; set; }

        public string? ProductJSON { get; set; }

        public string? Description { get; set; }
    }
}
