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
    /// Infrastructure for admin (global) notifications.
    /// Mirrors RoleInfrastructure style: no helper mappers, inline object inits.
    /// </summary>
    public class AdminNotificationInfrastructure : BaseInfrastructure, IAdminNotificationInfrastructure
    {
        #region Constructor
        public AdminNotificationInfrastructure(
            IConfiguration configuration,
            ILogger<AdminNotificationInfrastructure> logger)
            : base(configuration, logger)
        {
        }
        #endregion

        #region Constants - Stored Procedure Names
        private const string AddStoredProcedureName = "[dbo].[sp_AdminNotification_Add]";
        private const string GetAllStoredProcedureName = "[dbo].[sp_AdminNotification_GetAll]";
        private const string MarkAllReadStoredProcedureName = "[dbo].[sp_AdminNotification_MarkAllRead]";
        private const string ClearAllStoredProcedureName = "[dbo].[sp_AdminNotification_ClearAll]";
        private const string GetHistoryStoredProcedureName = "[dbo].[sp_AdminNotification_GetHistory]";
        #endregion

        #region Constants - Column Names
        private const string AdminNotificationIdColumnName = "AdminNotificationId";
        private const string TypeColumnName = "Type";
        private const string TitleColumnName = "Title";
        private const string MessageColumnName = "Message";
        private const string IsReadColumnName = "IsRead";
        private const string ReadDateColumnName = "ReadDate";
        private const string AffectedUserIdColumnName = "AffectedUserId";
        private const string AuctionIdColumnName = "AuctionId";
        private const string InventoryAuctionIdColumnName = "InventoryAuctionId";
        #endregion

        #region Constants - Parameter Names
        private const string AdminNotificationIdParameterName = "@AdminNotificationId";
        private const string TypeParameterName = "@Type";
        private const string TitleParameterName = "@Title";
        private const string MessageParameterName = "@Message";
        private const string IsReadParameterName = "@IsRead";
        private const string ReadDateParameterName = "@ReadDate";
        private const string CreatedByIdParameterName = "@CreatedById";
        private const string ModifiedByIdParameterName = "@ModifiedById";
        private const string UnreadOnlyParameterName = "@UnreadOnly";
        private const string TopParameterName = "@Top";
        private const string AffectedUserIdParameterName = "@AffectedUserId";
        private const string AuctionIdParameterName = "@AuctionId";
        private const string InventoryAuctionIdParameterName = "@InventoryAuctionId";
        #endregion

        #region IAdminNotificationInfrastructure

        /// <summary>
        /// Inserts a new admin notification and returns generated AdminNotificationId.
        /// </summary>
        public async Task<int> Add(AdminNotification entity)
        {
            // Prefer output param pattern (RoleInfrastructure style).
            var idOut = base.GetParameterOut(AdminNotificationIdParameterName, SqlDbType.Int, entity.AdminNotificationId);

            var parameters = new List<DbParameter>
            {
                idOut,
                base.GetParameter(TypeParameterName,               entity.Type ?? string.Empty),
                base.GetParameter(TitleParameterName,              entity.Title ?? string.Empty),
                base.GetParameter(MessageParameterName,            entity.Message ?? string.Empty),
                base.GetParameter(IsReadParameterName,             entity.IsRead),
                base.GetParameter(ReadDateParameterName,           entity.ReadDate.HasValue ? (object)entity.ReadDate.Value : DBNull.Value),
                base.GetParameter(CreatedByIdParameterName,        entity.CreatedById),
                base.GetParameter(AffectedUserIdParameterName,     entity.AffectedUserId.HasValue ? (object)entity.AffectedUserId.Value : DBNull.Value),
                base.GetParameter(AuctionIdParameterName,          entity.AuctionId.HasValue ? (object)entity.AuctionId.Value : DBNull.Value),
                base.GetParameter(InventoryAuctionIdParameterName, entity.InventoryAuctionId.HasValue ? (object)entity.InventoryAuctionId.Value : DBNull.Value)
            };

            await base.ExecuteNonQuery(parameters, AddStoredProcedureName, CommandType.StoredProcedure);

            entity.AdminNotificationId = Convert.ToInt32(idOut.Value);
            return entity.AdminNotificationId;
        }

        /// <summary>
        /// Returns admin notifications. When unreadOnly = true, only unread are returned.
        /// </summary>
        public async Task<List<AdminNotification>> GetList(bool unreadOnly = false, int top = 50)
        {
            var items = new List<AdminNotification>();

            var parameters = new List<DbParameter>
            {
                base.GetParameter(UnreadOnlyParameterName, unreadOnly),
                base.GetParameter(TopParameterName,        top)
            };

            using (var dr = await base.ExecuteReader(parameters, GetAllStoredProcedureName, CommandType.StoredProcedure))
            {
                if (dr != null)
                {
                    while (dr.Read())
                    {
                        var row = new AdminNotification
                        {
                            AdminNotificationId = dr.GetIntegerValue(AdminNotificationIdColumnName),
                            Type = dr.GetStringValue(TypeColumnName),
                            Title = dr.GetStringValue(TitleColumnName),
                            Message = dr.GetStringValue(MessageColumnName),
                            IsRead = dr.GetBooleanValue(IsReadColumnName),
                            ReadDate = dr.GetDateTimeValueNullable(ReadDateColumnName),

                            AffectedUserId = dr.GetIntegerValueNullable(AffectedUserIdColumnName),
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
        /// Marks all admin notifications as read and returns updated list.
        /// </summary>
        public async Task<List<AdminNotification>> MarkAllRead(int? modifiedById)
        {
            var items = new List<AdminNotification>();

            var parameters = new List<DbParameter>
            {
                base.GetParameter(ModifiedByIdParameterName, (object?)modifiedById ?? DBNull.Value)
            };

            using (var dr = await base.ExecuteReader(parameters, MarkAllReadStoredProcedureName, CommandType.StoredProcedure))
            {
                if (dr != null)
                {
                    while (dr.Read())
                    {
                        var row = new AdminNotification
                        {
                            AdminNotificationId = dr.GetIntegerValue(AdminNotificationIdColumnName),
                            Type = dr.GetStringValue(TypeColumnName),
                            Title = dr.GetStringValue(TitleColumnName),
                            Message = dr.GetStringValue(MessageColumnName),
                            IsRead = dr.GetBooleanValue(IsReadColumnName),
                            ReadDate = dr.GetDateTimeValueNullable(ReadDateColumnName),

                            AffectedUserId = dr.GetIntegerValueNullable(AffectedUserIdColumnName),
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
        /// Clears (soft-deletes) all admin notifications and returns remaining (usually empty).
        /// </summary>
        public async Task<List<AdminNotification>> ClearAll(int? modifiedById)
        {
            var items = new List<AdminNotification>();

            var parameters = new List<DbParameter>
            {
                base.GetParameter(ModifiedByIdParameterName, (object?)modifiedById ?? DBNull.Value)
            };

            using (var dr = await base.ExecuteReader(parameters, ClearAllStoredProcedureName, CommandType.StoredProcedure))
            {
                if (dr != null)
                {
                    while (dr.Read())
                    {
                        var row = new AdminNotification
                        {
                            AdminNotificationId = dr.GetIntegerValue(AdminNotificationIdColumnName),
                            Type = dr.GetStringValue(TypeColumnName),
                            Title = dr.GetStringValue(TitleColumnName),
                            Message = dr.GetStringValue(MessageColumnName),
                            IsRead = dr.GetBooleanValue(IsReadColumnName),
                            ReadDate = dr.GetDateTimeValueNullable(ReadDateColumnName),

                            AffectedUserId = dr.GetIntegerValueNullable(AffectedUserIdColumnName),
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
        /// Returns the historical list of admin notifications (independent of read/clear UI state).
        /// </summary>
        public async Task<List<AdminNotification>> GetHistory(int top = 200)
        {
            var items = new List<AdminNotification>();

            var parameters = new List<DbParameter>
            {
                base.GetParameter(TopParameterName, top)
            };

            using (var dr = await base.ExecuteReader(parameters, GetHistoryStoredProcedureName, CommandType.StoredProcedure))
            {
                if (dr != null)
                {
                    while (dr.Read())
                    {
                        var row = new AdminNotification
                        {
                            AdminNotificationId = dr.GetIntegerValue(AdminNotificationIdColumnName),
                            Type = dr.GetStringValue(TypeColumnName),
                            Title = dr.GetStringValue(TitleColumnName),
                            Message = dr.GetStringValue(MessageColumnName),
                            IsRead = dr.GetBooleanValue(IsReadColumnName),
                            ReadDate = dr.GetDateTimeValueNullable(ReadDateColumnName),

                            AffectedUserId = dr.GetIntegerValueNullable(AffectedUserIdColumnName),
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
