using AuctionPortal.Common.Models;

namespace AuctionPortal.Models
{
    public class InventoryInspector : BaseModel
    {
        public int InventoryInspectorId { get; set; }
        public int? AssignedTo { get; set; }
        public int InventoryId { get; set; }
        public string? InspectorName { get; set; }
    }
}
