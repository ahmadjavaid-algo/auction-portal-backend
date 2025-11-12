using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AuctionPortal.ApplicationLayer.IApplication;
using AuctionPortal.InfrastructureLayer.Interfaces;
using AuctionPortal.Models;
using Microsoft.Extensions.Configuration;

namespace AuctionPortal.ApplicationLayer.Application
{
    /// <summary>
    /// Application layer for admin (global) notifications.
    /// Thin wrapper over IAdminNotificationInfrastructure.
    /// </summary>
    public class AdminNotificationApplication : BaseApplication, IAdminNotificationApplication
    {
        public AdminNotificationApplication(
            IAdminNotificationInfrastructure infrastructure,
            IConfiguration configuration)
            : base(configuration)
        {
            Infrastructure = infrastructure ?? throw new ArgumentNullException(nameof(infrastructure));
        }

        public IAdminNotificationInfrastructure Infrastructure { get; }

        /// <summary>
        /// Adds a new admin notification and returns the generated AdminNotificationId.
        /// </summary>
        public Task<int> Add(AdminNotification entity) =>
            Infrastructure.Add(entity);

        /// <summary>
        /// Returns admin notifications (optionally unread-only).
        /// </summary>
        public Task<List<AdminNotification>> GetList(bool unreadOnly = false, int top = 50) =>
            Infrastructure.GetList(unreadOnly, top);

        /// <summary>
        /// Marks all admin notifications as read and returns the updated list.
        /// </summary>
        public Task<List<AdminNotification>> MarkAllRead(int? modifiedById) =>
            Infrastructure.MarkAllRead(modifiedById);

        /// <summary>
        /// Clears (soft-deletes) all admin notifications and returns the updated list.
        /// </summary>
        public Task<List<AdminNotification>> ClearAll(int? modifiedById) =>
            Infrastructure.ClearAll(modifiedById);
    }
}
