using AuctionPortal.Common.Models;

namespace AuctionPortal.Models
{
    public class Email: BaseModel
    {
        public int EmailId { get; set; }
        public string? EmailCode { get; set; }
        public string? EmailSubject { get; set; }
        public string? EmailBody { get; set; }
        public string? EmailTo { get; set; }
        public string? EmailFrom { get; set; }
        
    }
}
