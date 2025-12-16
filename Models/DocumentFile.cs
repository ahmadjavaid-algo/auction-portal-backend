using System;
using AuctionPortal.Common.Models;

namespace AuctionPortal.Models
{
    public class DocumentFile : BaseModel
    {
        public int DocumentFileId { get; set; }

        public string DocumentName { get; set; } = string.Empty;
        public int DocumentTypeId { get; set; }
        public string? DocumentThumbnailUrl { get; set; }
        public string? DocumentExtension { get; set; }   
        public string? DocumentUrl { get; set; }        
        public string? DocumentTypeName { get; set; }
    }
}
