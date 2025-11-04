using AuctionPortal.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuctionPortal.InfrastructureLayer.Interfaces
{
    public interface INotificationInfrastructure
    {
        Task<int> Add(Notification notification);

        Task<List<Notification>> GetForUser(int userId, bool unreadOnly = false, int top = 50);

        Task<List<Notification>> MarkAllRead(int userId, int? modifiedById);

        Task<List<Notification>> ClearAll(int userId, int? modifiedById);
    }
}
