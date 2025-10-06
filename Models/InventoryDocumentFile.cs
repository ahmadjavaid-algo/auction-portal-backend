using System;
using AuctionPortal.Common.Models;

namespace AuctionPortal.Models
{

    public class InventoryDocumentFile : BaseModel
    {
      
        public int InventoryDocumentFileId { get; set; }

        
        public int DocumentFileId { get; set; }
        public int InventoryId { get; set; }

        public string? DocumentDisplayName { get; set; }

        public string? DocumentName { get; set; }
        public int? DocumentTypeId { get; set; }
        public string? DocumentTypeName { get; set; }
        public string? DocumentExtension { get; set; }
        public string? DocumentUrl { get; set; }

        public string? InventoryDisplayName { get; set; }
        public string? ChassisNo { get; set; }
        public string? RegistrationNo { get; set; }
    }
}
