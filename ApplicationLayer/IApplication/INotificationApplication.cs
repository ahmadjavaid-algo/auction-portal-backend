// AuctionPortal.ApplicationLayer.IApplication/INotificationApplication.cs
using AuctionPortal.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuctionPortal.ApplicationLayer.IApplication
{
    public interface INotificationApplication
    {
        Task<int> Add(Notification entity);

        Task<List<Notification>> GetForUser(int userId, bool unreadOnly = false, int top = 50);

        Task<List<Notification>> MarkAllRead(int userId, int? modifiedById);

        Task<List<Notification>> ClearAll(int userId, int? modifiedById);
    }
}
