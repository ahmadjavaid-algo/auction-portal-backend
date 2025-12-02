using AuctionPortal.Common.Models;

namespace AuctionPortal.Models
{
    public class Inspection : BaseModel
    {
        public int InspectionId { get; set; }

        public int InspectionTypeId { get; set; }
        public string InspectionTypeName { get; set; } = string.Empty;

        public int InspectionCheckpointId { get; set; }
        public string InspectionCheckpointName { get; set; } = string.Empty;
        public string? InputType { get; set; }

        public int InventoryId { get; set; }
        public int ProductId { get; set; }
        public string ProductDisplayName { get; set; } = string.Empty;
        public string? ProductJSON { get; set; }
        public string? InventoryDescription { get; set; }

        public string? Result { get; set; }
    }
}
