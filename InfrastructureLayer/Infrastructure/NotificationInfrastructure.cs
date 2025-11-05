// AuctionPortal.InfrastructureLayer.Infrastructure/NotificationInfrastructure.cs
using AuctionPortal.Common.Infrastructure;
using AuctionPortal.InfrastructureLayer.Interfaces;
using AuctionPortal.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace AuctionPortal.InfrastructureLayer.Infrastructure
{
    /// <summary>
    /// Infrastructure for user notifications (Notification).
    /// </summary>
    public class NotificationInfrastructure : BaseInfrastructure, INotificationInfrastructure
    {
        #region Constructor

        public NotificationInfrastructure(IConfiguration configuration, ILogger<NotificationInfrastructure> logger)
            : base(configuration, logger)
        {
        }

        #endregion

        #region Constants – stored procedure names

        private const string AddStoredProcedureName = "[dbo].[sp_Notification_Add]";
        private const string GetForUserStoredProcedureName = "[dbo].[sp_Notification_GetForUser]";
        private const string MarkAllReadStoredProcedureName = "[dbo].[sp_Notification_MarkAllRead]";
        private const string ClearAllStoredProcedureName = "[dbo].[sp_Notification_ClearAll]";

        #endregion

        #region Constants – column names

        private const string NotificationIdColumnName = "NotificationId";
        private const string UserIdColumnName = "UserId";
        private const string TypeColumnName = "Type";
        private const string TitleColumnName = "Title";
        private const string MessageColumnName = "Message";
        private const string IsReadColumnName = "IsRead";
        private const string ReadDateColumnName = "ReadDate";
        private const string AuctionIdColumnName = "AuctionId";
        private const string InventoryAuctionIdColName = "InventoryAuctionId";

        #endregion

        #region Constants – parameter names

        private const string UserIdParameterName = "@UserId";
        private const string TypeParameterName = "@Type";
        private const string TitleParameterName = "@Title";
        private const string MessageParameterName = "@Message";
        private const string IsReadParameterName = "@IsRead";
        private const string ReadDateParameterName = "@ReadDate";
        private const string UnreadOnlyParameterName = "@UnreadOnly";
        private const string TopParameterName = "@Top";
        private const string CreatedByIdParameterName = "@CreatedById";
        private const string ModifiedByIdParameterName = "@ModifiedById";
        private const string AuctionIdParameterName = "@AuctionId";
        private const string InventoryAuctionIdParameterName = "@InventoryAuctionId";

        #endregion

        #region INotificationInfrastructure Implementation

        /// <summary>
        /// Add inserts a new notification and returns the generated NotificationId.
        /// </summary>
        public async Task<int> Add(Notification notification)
        {
            var parameters = new List<DbParameter>
            {
                GetParameter(UserIdParameterName,            notification.UserId),
                GetParameter(TypeParameterName,              notification.Type ?? string.Empty),
                GetParameter(TitleParameterName,             notification.Title ?? string.Empty),
                GetParameter(MessageParameterName,           notification.Message ?? string.Empty),
                GetParameter(IsReadParameterName,            notification.IsRead),
                GetParameter(ReadDateParameterName,
                    notification.ReadDate.HasValue
                        ? (object)notification.ReadDate.Value
                        : DBNull.Value),
                GetParameter(CreatedByIdParameterName,       notification.CreatedById),
                GetParameter(AuctionIdParameterName,
                    notification.AuctionId.HasValue
                        ? (object)notification.AuctionId.Value
                        : DBNull.Value),
                GetParameter(InventoryAuctionIdParameterName,
                    notification.InventoryAuctionId.HasValue
                        ? (object)notification.InventoryAuctionId.Value
                        : DBNull.Value)
            };

            using (var reader = await ExecuteReader(parameters, AddStoredProcedureName, CommandType.StoredProcedure))
            {
                if (reader != null && reader.HasRows && reader.Read())
                {
                    MapNotification(reader, notification);
                }

                if (reader != null && !reader.IsClosed)
                    reader.Close();
            }

            return notification.NotificationId;
        }

        /// <summary>
        /// GetForUser fetches notifications for a given user.
        /// </summary>
        public async Task<List<Notification>> GetForUser(int userId, bool unreadOnly = false, int top = 50)
        {
            var items = new List<Notification>();

            var parameters = new List<DbParameter>
            {
                GetParameter(UserIdParameterName,     userId),
                GetParameter(UnreadOnlyParameterName, unreadOnly),
                GetParameter(TopParameterName,        top)
            };

            using (var reader = await ExecuteReader(parameters, GetForUserStoredProcedureName, CommandType.StoredProcedure))
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        var item = new Notification();
                        MapNotification(reader, item);
                        items.Add(item);
                    }

                    if (!reader.IsClosed)
                        reader.Close();
                }
            }

            return items;
        }

        public async Task<List<Notification>> MarkAllRead(int userId, int? modifiedById)
        {
            var items = new List<Notification>();

            var parameters = new List<DbParameter>
            {
                GetParameter(UserIdParameterName,       userId),
                GetParameter(ModifiedByIdParameterName, (object?)modifiedById ?? DBNull.Value)
            };

            using (var reader = await ExecuteReader(parameters, MarkAllReadStoredProcedureName, CommandType.StoredProcedure))
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        var item = new Notification();
                        MapNotification(reader, item);
                        items.Add(item);
                    }

                    if (!reader.IsClosed)
                        reader.Close();
                }
            }

            return items;
        }

        public async Task<List<Notification>> ClearAll(int userId, int? modifiedById)
        {
            var items = new List<Notification>();

            var parameters = new List<DbParameter>
            {
                GetParameter(UserIdParameterName,       userId),
                GetParameter(ModifiedByIdParameterName, (object?)modifiedById ?? DBNull.Value)
            };

            using (var reader = await ExecuteReader(parameters, ClearAllStoredProcedureName, CommandType.StoredProcedure))
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        var item = new Notification();
                        MapNotification(reader, item);
                        items.Add(item);
                    }

                    if (!reader.IsClosed)
                        reader.Close();
                }
            }

            return items;
        }

        #endregion

        #region Helpers

        private static void MapNotification(DbDataReader reader, Notification target)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));

            target.NotificationId = reader.GetIntegerValue(NotificationIdColumnName);
            target.UserId = reader.GetIntegerValue(UserIdColumnName);
            target.Type = reader.GetStringValue(TypeColumnName);
            target.Title = reader.GetStringValue(TitleColumnName);
            target.Message = reader.GetStringValue(MessageColumnName);
            target.IsRead = reader.GetBooleanValue(IsReadColumnName);
            target.ReadDate = reader.GetDateTimeValueNullable(ReadDateColumnName);

            target.CreatedById = reader.GetIntegerValueNullable(BaseInfrastructure.CreatedByIdColumnName);
            target.CreatedDate = reader.GetDateTimeValueNullable(BaseInfrastructure.CreatedDateColumnName);
            target.ModifiedById = reader.GetIntegerValueNullable(BaseInfrastructure.ModifiedByIdColumnName) ?? 0;
            target.ModifiedDate = reader.GetDateTimeValueNullable(BaseInfrastructure.ModifiedDateColumnName);
            target.Active = reader.GetBooleanValue(BaseInfrastructure.ActiveColumnName);

            target.AuctionId = reader.GetIntegerValueNullable(AuctionIdColumnName);
            target.InventoryAuctionId = reader.GetIntegerValueNullable(InventoryAuctionIdColName);
        }

        #endregion
    }
}
