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
    /// Application layer for admin notifications
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

        public Task<int> Add(AdminNotification entity)
        {
            return Infrastructure.Add(entity);
        }

        public Task<List<AdminNotification>> GetList(bool unreadOnly = false, int top = 50)
        {
            return Infrastructure.GetList(unreadOnly, top);
        }

        public Task<List<AdminNotification>> MarkAllRead(int? modifiedById)
        {
            return Infrastructure.MarkAllRead(modifiedById);
        }

        public Task<List<AdminNotification>> ClearAll(int? modifiedById)
        {
            return Infrastructure.ClearAll(modifiedById);
        }

        public Task<List<AdminNotification>> GetHistory(int top = 200)
        {
            return Infrastructure.GetHistory(top);
        }
    }
}
