namespace AuctionPortal.Common.Models
{
    public class ApiRequestVM<T>
    {
        #region Request
        public T Data { get; set; } = default!;
        #endregion
    }
}
