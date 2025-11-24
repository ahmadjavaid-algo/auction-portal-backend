using System;
using AuctionPortal.Common.Models;

namespace AuctionPortal.Models
{
    public class Dashboard : BaseModel
    {
        public int DashboardstatsId { get; set; }
        public string? DashboardstatsName { get; set; }   
        public int? Dashboardnumber { get; set; }
    }
}
