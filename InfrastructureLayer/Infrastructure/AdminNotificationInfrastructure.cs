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
    /// Infrastructure for admin notifications (global).
    /// </summary>
    public class AdminNotificationInfrastructure : BaseInfrastructure, IAdminNotificationInfrastructure
    {
        public AdminNotificationInfrastructure(
            IConfiguration configuration,
            ILogger<AdminNotificationInfrastructure> logger)
            : base(configuration, logger)
        {
        }

        #region SP names

        private const string AddSpName = "[dbo].[sp_AdminNotification_Add]";
        private const string GetAllSpName = "[dbo].[sp_AdminNotification_GetAll]";
        private const string MarkAllReadSpName = "[dbo].[sp_AdminNotification_MarkAllRead]";
        private const string ClearAllSpName = "[dbo].[sp_AdminNotification_ClearAll]";

        #endregion

        #region Column names

        private const string IdCol = "AdminNotificationId";
        private const string TypeCol = "Type";
        private const string TitleCol = "Title";
        private const string MessageCol = "Message";
        private const string IsReadCol = "IsRead";
        private const string ReadDateCol = "ReadDate";
        private const string AffectedUserIdCol = "AffectedUserId";
        private const string AuctionIdCol = "AuctionId";
        private const string InventoryAuctionIdCol = "InventoryAuctionId";

        #endregion

        #region Parameter names

        private const string TypeParam = "@Type";
        private const string TitleParam = "@Title";
        private const string MessageParam = "@Message";
        private const string IsReadParam = "@IsRead";
        private const string ReadDateParam = "@ReadDate";
        private const string CreatedByIdParam = "@CreatedById";
        private const string ModifiedByIdParam = "@ModifiedById";
        private const string UnreadOnlyParam = "@UnreadOnly";
        private const string TopParam = "@Top";
        private const string AffectedUserIdParam = "@AffectedUserId";
        private const string AuctionIdParam = "@AuctionId";
        private const string InventoryAuctionIdParam = "@InventoryAuctionId";

        #endregion

        #region IAdminNotificationInfrastructure

        public async Task<int> Add(AdminNotification notification)
        {
            var parameters = new List<DbParameter>
            {
                GetParameter(TypeParam,    notification.Type ?? string.Empty),
                GetParameter(TitleParam,   notification.Title ?? string.Empty),
                GetParameter(MessageParam, notification.Message ?? string.Empty),
                GetParameter(IsReadParam,  notification.IsRead),
                GetParameter(ReadDateParam,
                    notification.ReadDate.HasValue
                        ? (object)notification.ReadDate.Value
                        : DBNull.Value),
                GetParameter(CreatedByIdParam, notification.CreatedById),
                GetParameter(AffectedUserIdParam,
                    notification.AffectedUserId.HasValue
                        ? (object)notification.AffectedUserId.Value
                        : DBNull.Value),
                GetParameter(AuctionIdParam,
                    notification.AuctionId.HasValue
                        ? (object)notification.AuctionId.Value
                        : DBNull.Value),
                GetParameter(InventoryAuctionIdParam,
                    notification.InventoryAuctionId.HasValue
                        ? (object)notification.InventoryAuctionId.Value
                        : DBNull.Value)
            };

            using (var reader = await ExecuteReader(parameters, AddSpName, CommandType.StoredProcedure))
            {
                if (reader != null && reader.HasRows && reader.Read())
                {
                    Map(reader, notification);
                }

                if (reader != null && !reader.IsClosed)
                    reader.Close();
            }

            return notification.AdminNotificationId;
        }

        public async Task<List<AdminNotification>> GetList(bool unreadOnly = false, int top = 50)
        {
            var items = new List<AdminNotification>();

            var parameters = new List<DbParameter>
            {
                GetParameter(UnreadOnlyParam, unreadOnly),
                GetParameter(TopParam,        top)
            };

            using (var reader = await ExecuteReader(parameters, GetAllSpName, CommandType.StoredProcedure))
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        var item = new AdminNotification();
                        Map(reader, item);
                        items.Add(item);
                    }

                    if (!reader.IsClosed)
                        reader.Close();
                }
            }

            return items;
        }

        public async Task<List<AdminNotification>> MarkAllRead(int? modifiedById)
        {
            var items = new List<AdminNotification>();

            var parameters = new List<DbParameter>
            {
                GetParameter(ModifiedByIdParam, (object?)modifiedById ?? DBNull.Value)
            };

            using (var reader = await ExecuteReader(parameters, MarkAllReadSpName, CommandType.StoredProcedure))
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        var item = new AdminNotification();
                        Map(reader, item);
                        items.Add(item);
                    }

                    if (!reader.IsClosed)
                        reader.Close();
                }
            }

            return items;
        }

        public async Task<List<AdminNotification>> ClearAll(int? modifiedById)
        {
            var items = new List<AdminNotification>();

            var parameters = new List<DbParameter>
            {
                GetParameter(ModifiedByIdParam, (object?)modifiedById ?? DBNull.Value)
            };

            using (var reader = await ExecuteReader(parameters, ClearAllSpName, CommandType.StoredProcedure))
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        var item = new AdminNotification();
                        Map(reader, item);
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

        private static void Map(DbDataReader reader, AdminNotification target)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));

            target.AdminNotificationId = reader.GetIntegerValue(IdCol);
            target.Type = reader.GetStringValue(TypeCol);
            target.Title = reader.GetStringValue(TitleCol);
            target.Message = reader.GetStringValue(MessageCol);
            target.IsRead = reader.GetBooleanValue(IsReadCol);
            target.ReadDate = reader.GetDateTimeValueNullable(ReadDateCol);

            target.AffectedUserId = reader.GetIntegerValueNullable(AffectedUserIdCol);
            target.AuctionId = reader.GetIntegerValueNullable(AuctionIdCol);
            target.InventoryAuctionId = reader.GetIntegerValueNullable(InventoryAuctionIdCol);

            target.CreatedById = reader.GetIntegerValueNullable(BaseInfrastructure.CreatedByIdColumnName);
            target.CreatedDate = reader.GetDateTimeValueNullable(BaseInfrastructure.CreatedDateColumnName);
            target.ModifiedById = reader.GetIntegerValueNullable(BaseInfrastructure.ModifiedByIdColumnName) ?? 0;
            target.ModifiedDate = reader.GetDateTimeValueNullable(BaseInfrastructure.ModifiedDateColumnName);
            target.Active = reader.GetBooleanValue(BaseInfrastructure.ActiveColumnName);
        }

        #endregion
    }
}
