using AuctionPortal.Common.Models;

namespace AuctionPortal.Models
{
    public class InspectionType : BaseModel
    {
        public int InspectionTypeId { get; set; }
        public string InspectionTypeName { get; set; } = string.Empty;
        public int Weightage { get; set; }
    }
}
