using System.Collections.Generic;
using System.Threading.Tasks;
using AuctionPortal.Models;

namespace AuctionPortal.InfrastructureLayer.Interfaces
{
    /// <summary>
    /// Infrastructure interface for admin (global) notifications.
    /// Note: does NOT inherit IBaseInfrastructure – it mirrors INotificationInfrastructure.
    /// </summary>
    public interface IAdminNotificationInfrastructure
    {
        Task<int> Add(AdminNotification notification);

        Task<List<AdminNotification>> GetList(bool unreadOnly = false, int top = 50);

        Task<List<AdminNotification>> MarkAllRead(int? modifiedById);

        Task<List<AdminNotification>> ClearAll(int? modifiedById);
        Task<List<AdminNotification>> GetHistory(int top = 200);
    }
}
