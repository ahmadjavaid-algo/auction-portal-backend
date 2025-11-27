using AuctionPortal.Common.Models;

namespace AuctionPortal.Models
{
    public class InspectionCheckpoint : BaseModel
    {
        public int InspectionCheckpointId { get; set; }
        public int InspectionTypeId { get; set; }
        public string InspectionCheckpointName { get; set; } = string.Empty;
        public string InputType { get; set; } = string.Empty;

        
        public string? InspectionTypeName { get; set; }
    }
}
