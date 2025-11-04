// AuctionPortal.ApplicationLayer.Application/NotificationApplication.cs
using AuctionPortal.ApplicationLayer.IApplication;
using AuctionPortal.InfrastructureLayer.Interfaces;
using AuctionPortal.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuctionPortal.ApplicationLayer.Application
{
    /// <summary>
    /// Application layer for user notifications.
    /// Thin wrapper over INotificationInfrastructure.
    /// </summary>
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

        /// <summary>
        /// Adds a new notification and returns the generated NotificationId.
        /// </summary>
        public async Task<int> Add(Notification entity)
        {
            return await NotificationInfrastructure.Add(entity);
        }

        #endregion

        #region Queries

        /// <summary>
        /// Returns notifications for the given user.
        /// </summary>
        /// <param name="userId">User whose notifications to fetch.</param>
        /// <param name="unreadOnly">If true, only unread notifications are returned.</param>
        /// <param name="top">Maximum number of records to return.</param>
        public async Task<List<Notification>> GetForUser(int userId, bool unreadOnly = false, int top = 50)
        {
            return await NotificationInfrastructure.GetForUser(userId, unreadOnly, top);
        }

        /// <summary>
        /// Marks all notifications for the user as read and returns the updated list.
        /// </summary>
        public async Task<List<Notification>> MarkAllRead(int userId, int? modifiedById)
        {
            return await NotificationInfrastructure.MarkAllRead(userId, modifiedById);
        }

        /// <summary>
        /// Clears (soft-deletes) all notifications for the user and returns the updated list.
        /// </summary>
        public async Task<List<Notification>> ClearAll(int userId, int? modifiedById)
        {
            return await NotificationInfrastructure.ClearAll(userId, modifiedById);
        }

        #endregion
    }
}
