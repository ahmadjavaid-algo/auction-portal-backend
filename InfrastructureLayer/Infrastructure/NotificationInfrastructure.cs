using AuctionPortal.Common.Infrastructure;
using AuctionPortal.InfrastructureLayer.Interfaces;
using AuctionPortal.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Data.Common;

namespace AuctionPortal.InfrastructureLayer.Infrastructure
{
    /// <summary>
    /// Infrastructure for user notifications.
    /// Mirrors RoleInfrastructure style (no helpers; inline mapping; output param on Add).
    /// </summary>
    public class NotificationInfrastructure : BaseInfrastructure, INotificationInfrastructure
    {
        #region Constructor
        public NotificationInfrastructure(
            IConfiguration configuration,
            ILogger<NotificationInfrastructure> logger)
            : base(configuration, logger)
        {
        }
        #endregion

        #region Constants – Stored Procedure Names
        private const string AddStoredProcedureName = "[dbo].[sp_Notification_Add]";
        private const string GetForUserStoredProcedureName = "[dbo].[sp_Notification_GetForUser]";
        private const string MarkAllReadStoredProcedureName = "[dbo].[sp_Notification_MarkAllRead]";
        private const string ClearAllStoredProcedureName = "[dbo].[sp_Notification_ClearAll]";
        #endregion

        #region Constants – Column Names
        private const string NotificationIdColumnName = "NotificationId";
        private const string UserIdColumnName = "UserId";
        private const string TypeColumnName = "Type";
        private const string TitleColumnName = "Title";
        private const string MessageColumnName = "Message";
        private const string IsReadColumnName = "IsRead";
        private const string ReadDateColumnName = "ReadDate";
        private const string AuctionIdColumnName = "AuctionId";
        private const string InventoryAuctionIdColumnName = "InventoryAuctionId";
        #endregion

        #region Constants – Parameter Names
        private const string NotificationIdParameterName = "@NotificationId";
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
        /// Inserts a new notification and returns generated NotificationId.
        /// </summary>
        public async Task<int> Add(Notification notification)
        {
            var idOut = base.GetParameterOut(NotificationIdParameterName, SqlDbType.Int, notification.NotificationId);

            var parameters = new List<DbParameter>
            {
                idOut,
                base.GetParameter(UserIdParameterName,              notification.UserId),
                base.GetParameter(TypeParameterName,                notification.Type ?? string.Empty),
                base.GetParameter(TitleParameterName,               notification.Title ?? string.Empty),
                base.GetParameter(MessageParameterName,             notification.Message ?? string.Empty),
                base.GetParameter(IsReadParameterName,              notification.IsRead),
                base.GetParameter(ReadDateParameterName,            notification.ReadDate.HasValue ? (object)notification.ReadDate.Value : DBNull.Value),
                base.GetParameter(CreatedByIdParameterName,         notification.CreatedById),
                base.GetParameter(AuctionIdParameterName,           notification.AuctionId.HasValue ? (object)notification.AuctionId.Value : DBNull.Value),
                base.GetParameter(InventoryAuctionIdParameterName,  notification.InventoryAuctionId.HasValue ? (object)notification.InventoryAuctionId.Value : DBNull.Value)
            };

            await base.ExecuteNonQuery(parameters, AddStoredProcedureName, CommandType.StoredProcedure);

            notification.NotificationId = Convert.ToInt32(idOut.Value);
            return notification.NotificationId;
        }

        /// <summary>
        /// Returns notifications for a user (optionally only unread; limited by top).
        /// </summary>
        public async Task<List<Notification>> GetForUser(int userId, bool unreadOnly = false, int top = 50)
        {
            var items = new List<Notification>();

            var parameters = new List<DbParameter>
            {
                base.GetParameter(UserIdParameterName,     userId),
                base.GetParameter(UnreadOnlyParameterName, unreadOnly),
                base.GetParameter(TopParameterName,        top)
            };

            using (var dr = await base.ExecuteReader(parameters, GetForUserStoredProcedureName, CommandType.StoredProcedure))
            {
                if (dr != null)
                {
                    while (dr.Read())
                    {
                        var row = new Notification
                        {
                            NotificationId = dr.GetIntegerValue(NotificationIdColumnName),
                            UserId = dr.GetIntegerValue(UserIdColumnName),
                            Type = dr.GetStringValue(TypeColumnName),
                            Title = dr.GetStringValue(TitleColumnName),
                            Message = dr.GetStringValue(MessageColumnName),
                            IsRead = dr.GetBooleanValue(IsReadColumnName),
                            ReadDate = dr.GetDateTimeValueNullable(ReadDateColumnName),

                            AuctionId = dr.GetIntegerValueNullable(AuctionIdColumnName),
                            InventoryAuctionId = dr.GetIntegerValueNullable(InventoryAuctionIdColumnName),

                            CreatedById = dr.GetIntegerValueNullable(BaseInfrastructure.CreatedByIdColumnName),
                            CreatedDate = dr.GetDateTimeValueNullable(BaseInfrastructure.CreatedDateColumnName),
                            ModifiedById = dr.GetIntegerValueNullable(BaseInfrastructure.ModifiedByIdColumnName) ?? 0,
                            ModifiedDate = dr.GetDateTimeValueNullable(BaseInfrastructure.ModifiedDateColumnName),
                            Active = dr.GetBooleanValue(BaseInfrastructure.ActiveColumnName)
                        };

                        items.Add(row);
                    }

                    if (!dr.IsClosed) dr.Close();
                }
            }

            return items;
        }

        /// <summary>
        /// Marks all notifications for a user as read; returns the (updated) list per SP behavior.
        /// </summary>
        public async Task<List<Notification>> MarkAllRead(int userId, int? modifiedById)
        {
            var items = new List<Notification>();

            var parameters = new List<DbParameter>
            {
                base.GetParameter(UserIdParameterName,       userId),
                base.GetParameter(ModifiedByIdParameterName, (object?)modifiedById ?? DBNull.Value)
            };

            using (var dr = await base.ExecuteReader(parameters, MarkAllReadStoredProcedureName, CommandType.StoredProcedure))
            {
                if (dr != null)
                {
                    while (dr.Read())
                    {
                        var row = new Notification
                        {
                            NotificationId = dr.GetIntegerValue(NotificationIdColumnName),
                            UserId = dr.GetIntegerValue(UserIdColumnName),
                            Type = dr.GetStringValue(TypeColumnName),
                            Title = dr.GetStringValue(TitleColumnName),
                            Message = dr.GetStringValue(MessageColumnName),
                            IsRead = dr.GetBooleanValue(IsReadColumnName),
                            ReadDate = dr.GetDateTimeValueNullable(ReadDateColumnName),

                            AuctionId = dr.GetIntegerValueNullable(AuctionIdColumnName),
                            InventoryAuctionId = dr.GetIntegerValueNullable(InventoryAuctionIdColumnName),

                            CreatedById = dr.GetIntegerValueNullable(BaseInfrastructure.CreatedByIdColumnName),
                            CreatedDate = dr.GetDateTimeValueNullable(BaseInfrastructure.CreatedDateColumnName),
                            ModifiedById = dr.GetIntegerValueNullable(BaseInfrastructure.ModifiedByIdColumnName) ?? 0,
                            ModifiedDate = dr.GetDateTimeValueNullable(BaseInfrastructure.ModifiedDateColumnName),
                            Active = dr.GetBooleanValue(BaseInfrastructure.ActiveColumnName)
                        };

                        items.Add(row);
                    }

                    if (!dr.IsClosed) dr.Close();
                }
            }

            return items;
        }

        /// <summary>
        /// Clears (soft-deletes) all notifications for a user; returns list per SP behavior.
        /// </summary>
        public async Task<List<Notification>> ClearAll(int userId, int? modifiedById)
        {
            var items = new List<Notification>();

            var parameters = new List<DbParameter>
            {
                base.GetParameter(UserIdParameterName,       userId),
                base.GetParameter(ModifiedByIdParameterName, (object?)modifiedById ?? DBNull.Value)
            };

            using (var dr = await base.ExecuteReader(parameters, ClearAllStoredProcedureName, CommandType.StoredProcedure))
            {
                if (dr != null)
                {
                    while (dr.Read())
                    {
                        var row = new Notification
                        {
                            NotificationId = dr.GetIntegerValue(NotificationIdColumnName),
                            UserId = dr.GetIntegerValue(UserIdColumnName),
                            Type = dr.GetStringValue(TypeColumnName),
                            Title = dr.GetStringValue(TitleColumnName),
                            Message = dr.GetStringValue(MessageColumnName),
                            IsRead = dr.GetBooleanValue(IsReadColumnName),
                            ReadDate = dr.GetDateTimeValueNullable(ReadDateColumnName),

                            AuctionId = dr.GetIntegerValueNullable(AuctionIdColumnName),
                            InventoryAuctionId = dr.GetIntegerValueNullable(InventoryAuctionIdColumnName),

                            CreatedById = dr.GetIntegerValueNullable(BaseInfrastructure.CreatedByIdColumnName),
                            CreatedDate = dr.GetDateTimeValueNullable(BaseInfrastructure.CreatedDateColumnName),
                            ModifiedById = dr.GetIntegerValueNullable(BaseInfrastructure.ModifiedByIdColumnName) ?? 0,
                            ModifiedDate = dr.GetDateTimeValueNullable(BaseInfrastructure.ModifiedDateColumnName),
                            Active = dr.GetBooleanValue(BaseInfrastructure.ActiveColumnName)
                        };

                        items.Add(row);
                    }

                    if (!dr.IsClosed) dr.Close();
                }
            }

            return items;
        }

        #endregion
    }
}
