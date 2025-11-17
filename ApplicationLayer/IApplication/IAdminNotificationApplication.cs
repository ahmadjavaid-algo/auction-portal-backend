using System.Collections.Generic;
using System.Threading.Tasks;
using AuctionPortal.Models;

namespace AuctionPortal.ApplicationLayer.IApplication
{
    /// <summary>
    /// Application interface for admin (global) notifications.
    /// Note: does NOT inherit IBaseApplication – it mirrors INotificationApplication.
    /// </summary>
    public interface IAdminNotificationApplication
    {
        Task<int> Add(AdminNotification entity);

        Task<List<AdminNotification>> GetList(bool unreadOnly = false, int top = 50);

        Task<List<AdminNotification>> MarkAllRead(int? modifiedById);

        Task<List<AdminNotification>> ClearAll(int? modifiedById);
        Task<List<AdminNotification>> GetHistory(int top = 200);
    }
}
