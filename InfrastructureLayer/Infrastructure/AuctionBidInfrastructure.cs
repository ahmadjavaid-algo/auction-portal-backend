// AuctionPortal.InfrastructureLayer.Infrastructure/AuctionBidInfrastructure.cs
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
    /// Infrastructure for auction bids (AuctionBid).
    /// </summary>
    public class AuctionBidInfrastructure : BaseInfrastructure, IAuctionBidInfrastructure
    {
        #region Constructor

        public AuctionBidInfrastructure(
            IConfiguration configuration,
            ILogger<AuctionBidInfrastructure> logger)
            : base(configuration, logger)
        {
        }

        #endregion

        #region Constants – stored procedure names

        private const string AddStoredProcedureName = "[dbo].[sp_AuctionBid_Add]";
        private const string ActivateStoredProcedureName = "[dbo].[sp_AuctionBid_Activate]";
        private const string GetStoredProcedureName = "[dbo].[sp_AuctionBid_Get]";
        private const string GetListStoredProcedureName = "[dbo].[sp_AuctionBid_GetAll]";
        private const string UpdateStoredProcedureName = "[dbo].[sp_AuctionBid_Update]";

        #endregion

        #region Constants – column names

        private const string AuctionBidIdColumnName = "AuctionBidId";
        private const string AuctionIdColumnName = "AuctionId";
        private const string AuctionBidStatusIdColumnName = "AuctionBidStatusId";
        private const string InventoryAuctionIdColumnName = "InventoryAuctionId";
        private const string BidAmountColumnName = "BidAmount";
        private const string AuctionBidStatusNameColumn = "AuctionBidStatusName";

       
        private const string IsAutoBidColumnName = "IsAutoBid";
        private const string AutoBidAmountColumnName = "AutoBidAmount";

        #endregion

        #region Constants – parameter names

        private const string AuctionBidIdParameterName = "@AuctionBidId";
        private const string AuctionIdParameterName = "@AuctionId";
        private const string AuctionBidStatusIdParameterName = "@AuctionBidStatusId";
        private const string InventoryAuctionIdParameterName = "@InventoryAuctionId";
        private const string BidAmountParameterName = "@BidAmount";
        private const string CreatedByIdParameterName = "@CreatedById";
        private const string ModifiedByIdParameterName = "@ModifiedById";

        private const string IsAutoBidParameterName = "@IsAutoBid";
        private const string AutoBidAmountParameterName = "@AutoBidAmount";

        #endregion

        #region IAuctionBidInfrastructure implementation

        /// <summary>
        /// Add inserts a new AuctionBid row and returns the generated AuctionBidId.
        /// </summary>
        public async Task<int> Add(AuctionBid bid)
        {
            var parameters = new List<DbParameter>
            {
                base.GetParameter(AuctionIdParameterName,          bid.AuctionId),
                base.GetParameter(InventoryAuctionIdParameterName, bid.InventoryAuctionId),
                base.GetParameter(BidAmountParameterName,          bid.BidAmount),
                base.GetParameter(CreatedByIdParameterName,        bid.CreatedById),

              
                base.GetParameter(IsAutoBidParameterName,          bid.IsAutoBid),

                base.GetParameter(
                    AutoBidAmountParameterName,
                    bid.AutoBidAmount.HasValue && bid.AutoBidAmount.Value > 0
                        ? (object)bid.AutoBidAmount.Value
                        : DBNull.Value)
            };

            using (var reader = await base.ExecuteReader(parameters, AddStoredProcedureName, CommandType.StoredProcedure))
            {
                if (reader != null && reader.HasRows && reader.Read())
                {
                    bid.AuctionBidId = reader.GetIntegerValue(AuctionBidIdColumnName);
                    bid.AuctionId = reader.GetIntegerValue(AuctionIdColumnName);
                    bid.AuctionBidStatusId = reader.GetIntegerValue(AuctionBidStatusIdColumnName);
                    bid.InventoryAuctionId = reader.GetIntegerValue(InventoryAuctionIdColumnName);
                    bid.BidAmount = reader.GetDecimalValue(BidAmountColumnName);

                    bid.AuctionBidStatusName = reader.GetStringValue(AuctionBidStatusNameColumn);

                   
                    bid.IsAutoBid = reader.GetBooleanValue(IsAutoBidColumnName);
                    bid.AutoBidAmount = reader.GetIntegerValueNullable(AutoBidAmountColumnName);

                    bid.CreatedById = reader.GetIntegerValueNullable(BaseInfrastructure.CreatedByIdColumnName);
                    bid.CreatedDate = reader.GetDateTimeValueNullable(BaseInfrastructure.CreatedDateColumnName);
                    bid.ModifiedById = reader.GetIntegerValueNullable(BaseInfrastructure.ModifiedByIdColumnName) ?? 0;
                    bid.ModifiedDate = reader.GetDateTimeValueNullable(BaseInfrastructure.ModifiedDateColumnName);
                    bid.Active = reader.GetBooleanValue(BaseInfrastructure.ActiveColumnName);
                }

                if (reader != null && !reader.IsClosed)
                    reader.Close();
            }

            return bid.AuctionBidId;
        }

        /// <summary>
        /// Activate / deactivate a bid (soft delete).
        /// </summary>
        public async Task<bool> Activate(AuctionBid bid)
        {
            var parameters = new List<DbParameter>
            {
                base.GetParameter(AuctionBidIdParameterName,            bid.AuctionBidId),
                base.GetParameter(BaseInfrastructure.ActiveParameterName, bid.Active),
                base.GetParameter(ModifiedByIdParameterName,            bid.ModifiedById)
            };

            var rows = await base.ExecuteNonQuery(parameters, ActivateStoredProcedureName, CommandType.StoredProcedure);
            return rows > 0;
        }

        /// <summary>
        /// Get fetches and returns a single AuctionBid by id.
        /// </summary>
        public async Task<AuctionBid> Get(AuctionBid bid)
        {
            AuctionBid item = null;

            var parameters = new List<DbParameter>
            {
                base.GetParameter(AuctionBidIdParameterName, bid.AuctionBidId)
            };

            using (var reader = await base.ExecuteReader(parameters, GetStoredProcedureName, CommandType.StoredProcedure))
            {
                if (reader != null && reader.HasRows && reader.Read())
                {
                    item = new AuctionBid
                    {
                        AuctionBidId = reader.GetIntegerValue(AuctionBidIdColumnName),
                        AuctionId = reader.GetIntegerValue(AuctionIdColumnName),
                        AuctionBidStatusId = reader.GetIntegerValue(AuctionBidStatusIdColumnName),
                        InventoryAuctionId = reader.GetIntegerValue(InventoryAuctionIdColumnName),
                        BidAmount = reader.GetDecimalValue(BidAmountColumnName),

                        AuctionBidStatusName = reader.GetStringValue(AuctionBidStatusNameColumn),

                       
                        IsAutoBid = reader.GetBooleanValue(IsAutoBidColumnName),
                        AutoBidAmount = reader.GetIntegerValueNullable(AutoBidAmountColumnName),

                        CreatedById = reader.GetIntegerValueNullable(BaseInfrastructure.CreatedByIdColumnName),
                        CreatedDate = reader.GetDateTimeValueNullable(BaseInfrastructure.CreatedDateColumnName),
                        ModifiedById = reader.GetIntegerValueNullable(BaseInfrastructure.ModifiedByIdColumnName) ?? 0,
                        ModifiedDate = reader.GetDateTimeValueNullable(BaseInfrastructure.ModifiedDateColumnName),
                        Active = reader.GetBooleanValue(BaseInfrastructure.ActiveColumnName)
                    };
                }

                if (reader != null && !reader.IsClosed)
                    reader.Close();
            }

            return item;
        }

        /// <summary>
        /// GetList fetches and returns a list of bids.
        /// </summary>
        public async Task<List<AuctionBid>> GetList(AuctionBid _)
        {
            var items = new List<AuctionBid>();
            var parameters = new List<DbParameter>(); // none

            using (var reader = await base.ExecuteReader(parameters, GetListStoredProcedureName, CommandType.StoredProcedure))
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        var item = new AuctionBid
                        {
                            AuctionBidId = reader.GetIntegerValue(AuctionBidIdColumnName),
                            AuctionId = reader.GetIntegerValue(AuctionIdColumnName),
                            AuctionBidStatusId = reader.GetIntegerValue(AuctionBidStatusIdColumnName),
                            InventoryAuctionId = reader.GetIntegerValue(InventoryAuctionIdColumnName),
                            BidAmount = reader.GetDecimalValue(BidAmountColumnName),

                            AuctionBidStatusName = reader.GetStringValue(AuctionBidStatusNameColumn),

                            
                            IsAutoBid = reader.GetBooleanValue(IsAutoBidColumnName),
                            AutoBidAmount = reader.GetIntegerValueNullable(AutoBidAmountColumnName),

                            CreatedById = reader.GetIntegerValueNullable(BaseInfrastructure.CreatedByIdColumnName),
                            CreatedDate = reader.GetDateTimeValueNullable(BaseInfrastructure.CreatedDateColumnName),
                            ModifiedById = reader.GetIntegerValueNullable(BaseInfrastructure.ModifiedByIdColumnName) ?? 0,
                            ModifiedDate = reader.GetDateTimeValueNullable(BaseInfrastructure.ModifiedDateColumnName),
                            Active = reader.GetBooleanValue(BaseInfrastructure.ActiveColumnName)
                        };

                        items.Add(item);
                    }

                    if (!reader.IsClosed)
                        reader.Close();
                }
            }

            return items;
        }

        /// <summary>
        /// Update updates an existing bid and returns true if successful.
        /// </summary>
        public async Task<bool> Update(AuctionBid bid)
        {
            var parameters = new List<DbParameter>
            {
                base.GetParameter(AuctionBidIdParameterName, bid.AuctionBidId),

                base.GetParameter(
                    AuctionIdParameterName,
                    bid.AuctionId > 0 ? (object)bid.AuctionId : DBNull.Value),

                base.GetParameter(
                    InventoryAuctionIdParameterName,
                    bid.InventoryAuctionId > 0 ? (object)bid.InventoryAuctionId : DBNull.Value),

                base.GetParameter(
                    AuctionBidStatusIdParameterName,
                    bid.AuctionBidStatusId > 0 ? (object)bid.AuctionBidStatusId : DBNull.Value),

                base.GetParameter(
                    BidAmountParameterName,
                    bid.BidAmount >= 0 ? (object)bid.BidAmount : DBNull.Value),

                base.GetParameter(ModifiedByIdParameterName, bid.ModifiedById)


            };

            var rows = await base.ExecuteNonQuery(parameters, UpdateStoredProcedureName, CommandType.StoredProcedure);
            return rows > 0;
        }

        #endregion
    }
}
