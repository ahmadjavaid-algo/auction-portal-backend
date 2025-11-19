
using AuctionPortal.ApplicationLayer.IApplication;
using AuctionPortal.InfrastructureLayer.Interfaces;
using AuctionPortal.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuctionPortal.ApplicationLayer.Application
{

    public class NotificationApplication : BaseApplication, INotificationApplication
    {
        public NotificationApplication(
            INotificationInfrastructure notificationInfrastructure,
            IConfiguration configuration)
            : base(configuration)
        {
            NotificationInfrastructure = notificationInfrastructure
                ?? throw new ArgumentNullException(nameof(notificationInfrastructure));
        }

        public INotificationInfrastructure NotificationInfrastructure { get; }

        #region Commands


        public async Task<int> Add(Notification entity)
        {
            return await NotificationInfrastructure.Add(entity);
        }

        #endregion

        #region Queries


        public async Task<List<Notification>> GetForUser(int userId, bool unreadOnly = false, int top = 50)
        {
            return await NotificationInfrastructure.GetForUser(userId, unreadOnly, top);
        }


        public async Task<List<Notification>> MarkAllRead(int userId, int? modifiedById)
        {
            return await NotificationInfrastructure.MarkAllRead(userId, modifiedById);
        }

        public async Task<List<Notification>> ClearAll(int userId, int? modifiedById)
        {
            return await NotificationInfrastructure.ClearAll(userId, modifiedById);
        }

        #endregion
    }
}
