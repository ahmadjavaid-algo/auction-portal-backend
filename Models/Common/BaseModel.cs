namespace AuctionPortal.Common.Models
{
    public abstract class BaseModel
    {
        #region Audit
        public int? CreatedById { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int ModifiedById { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool Active { get; set; } = true;
        #endregion
    }
}
