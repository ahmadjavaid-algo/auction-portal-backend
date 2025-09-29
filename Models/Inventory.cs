using AuctionPortal.Common.Models;

namespace AuctionPortal.Models
{
    public class Inventory:BaseModel
    {
        public int InventoryId { get; set; }
        public int ProductId { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string? ProductJSON { get; set; }

        public string? Description { get; set; }
        public string? ChassisNo { get; set; }
        public string? RegistrationNo { get; set; }
    }
}
