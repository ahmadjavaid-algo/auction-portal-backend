// AuctionPortal.InfrastructureLayer.Infrastructure/InventoryAuctionInfrastructure.cs
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
    public class InventoryAuctionInfrastructure : BaseInfrastructure, IInventoryAuctionInfrastructure
    {
        #region Constructor
        /// <summary>
        /// InventoryAuctionInfrastructure initializes class object.
        /// </summary>
        public InventoryAuctionInfrastructure(IConfiguration configuration, ILogger<InventoryAuctionInfrastructure> logger)
            : base(configuration, logger)
        {
        }
        #endregion

        #region Constants
        private const string AddStoredProcedureName = "[dbo].[sp_InventoryAuction_Add]";
        private const string ActivateStoredProcedureName = "[dbo].[sp_InventoryAuction_Activate]";
        private const string GetStoredProcedureName = "[dbo].[sp_InventoryAuction_Get]";
        private const string GetListStoredProcedureName = "[dbo].[sp_InventoryAuction_GetAll]";
        private const string UpdateStoredProcedureName = "[dbo].[sp_InventoryAuction_Update]";

        // Column names returned by the SPs
        private const string InventoryAuctionIdColumnName = "InventoryAuctionId";
        private const string InventoryAuctionStatusIdColumnName = "InventoryAuctionStatusId";
        private const string InventoryIdColumnName = "InventoryId";
        private const string AuctionIdColumnName = "AuctionId";
        private const string BidIncrementColumnName = "BidIncrement";
        private const string BuyNowPriceColumnName = "BuyNowPrice";
        private const string ReservePriceColumnName = "ReservePrice";
        private const string InventoryAuctionStatusCodeColumn = "InventoryAuctionStatusCode";
        private const string InventoryAuctionStatusNameColumn = "InventoryAuctionStatusName";

        // Parameter names (match SP signatures)
        private const string InventoryAuctionIdParameterName = "@InventoryAuctionId";
        private const string InventoryAuctionStatusIdParameterName = "@InventoryAuctionStatusId";
        private const string InventoryIdParameterName = "@InventoryId";
        private const string AuctionIdParameterName = "@AuctionId";
        private const string BidIncrementParameterName = "@BidIncrement";
        private const string BuyNowPriceParameterName = "@BuyNowPrice";
        private const string ReservePriceParameterName = "@ReservePrice";
        private const string CreatedByIdParameterName = "@CreatedById";
        private const string ModifiedByIdParameterName = "@ModifiedById";
        #endregion

        #region IInventoryAuctionInfrastructure Implementation

        /// <summary>
        /// Add adds new InventoryAuction and returns generated InventoryAuctionId.
        /// BidIncrement is snapshotted inside the SP from Auction.AuctionId.
        /// </summary>
        public async Task<int> Add(InventoryAuction invAuc)
        {
            var parameters = new List<DbParameter>
            {
                base.GetParameter(InventoryAuctionStatusIdParameterName, invAuc.InventoryAuctionStatusId),
                base.GetParameter(InventoryIdParameterName,              invAuc.InventoryId),
                base.GetParameter(AuctionIdParameterName,                invAuc.AuctionId),
                base.GetParameter(BuyNowPriceParameterName,              (object?)invAuc.BuyNowPrice   ?? DBNull.Value),
                base.GetParameter(ReservePriceParameterName,             (object?)invAuc.ReservePrice  ?? DBNull.Value),
                base.GetParameter(CreatedByIdParameterName,              invAuc.CreatedById)
            };

            using (var reader = await base.ExecuteReader(parameters, AddStoredProcedureName, CommandType.StoredProcedure))
            {
                if (reader != null && reader.HasRows && reader.Read())
                {
                    invAuc.InventoryAuctionId = reader.GetIntegerValue(InventoryAuctionIdColumnName);
                    invAuc.InventoryAuctionStatusId = reader.GetIntegerValue(InventoryAuctionStatusIdColumnName);
                    invAuc.InventoryId = reader.GetIntegerValue(InventoryIdColumnName);
                    invAuc.AuctionId = reader.GetIntegerValue(AuctionIdColumnName);
                    invAuc.BidIncrement = reader.GetIntegerValue(BidIncrementColumnName);
                    invAuc.BuyNowPrice = reader.GetIntegerValueNullable(BuyNowPriceColumnName) ?? 0;
                    invAuc.ReservePrice = reader.GetIntegerValueNullable(ReservePriceColumnName) ?? 0;

                    invAuc.InventoryAuctionStatusCode = reader.GetStringValue(InventoryAuctionStatusCodeColumn);
                    invAuc.InventoryAuctionStatusName = reader.GetStringValue(InventoryAuctionStatusNameColumn);

                    invAuc.CreatedById = reader.GetIntegerValueNullable(BaseInfrastructure.CreatedByIdColumnName);
                    invAuc.CreatedDate = reader.GetDateTimeValueNullable(BaseInfrastructure.CreatedDateColumnName);
                    invAuc.ModifiedById = reader.GetIntegerValueNullable(BaseInfrastructure.ModifiedByIdColumnName) ?? 0;
                    invAuc.ModifiedDate = reader.GetDateTimeValueNullable(BaseInfrastructure.ModifiedDateColumnName);
                    invAuc.Active = reader.GetBooleanValue(BaseInfrastructure.ActiveColumnName);
                }

                if (reader != null && !reader.IsClosed)
                    reader.Close();
            }

            return invAuc.InventoryAuctionId;
        }

        /// <summary>
        /// Activate activate/deactivate provided record and returns true if successful.
        /// </summary>
        public async Task<bool> Activate(InventoryAuction invAuc)
        {
            var parameters = new List<DbParameter>
            {
                base.GetParameter(InventoryAuctionIdParameterName, invAuc.InventoryAuctionId),
                base.GetParameter(BaseInfrastructure.ActiveParameterName, invAuc.Active),
                base.GetParameter(ModifiedByIdParameterName, invAuc.ModifiedById)
            };

            var rows = await base.ExecuteNonQuery(parameters, ActivateStoredProcedureName, CommandType.StoredProcedure);
            return rows > 0;
        }

        /// <summary>
        /// Get fetches and returns a single InventoryAuction.
        /// </summary>
        public async Task<InventoryAuction> Get(InventoryAuction invAuc)
        {
            InventoryAuction item = null;

            var parameters = new List<DbParameter>
            {
                base.GetParameter(InventoryAuctionIdParameterName, invAuc.InventoryAuctionId)
            };

            using (var reader = await base.ExecuteReader(parameters, GetStoredProcedureName, CommandType.StoredProcedure))
            {
                if (reader != null && reader.HasRows && reader.Read())
                {
                    item = new InventoryAuction
                    {
                        InventoryAuctionId = reader.GetIntegerValue(InventoryAuctionIdColumnName),
                        InventoryAuctionStatusId = reader.GetIntegerValue(InventoryAuctionStatusIdColumnName),
                        InventoryId = reader.GetIntegerValue(InventoryIdColumnName),
                        AuctionId = reader.GetIntegerValue(AuctionIdColumnName),
                        BidIncrement = reader.GetIntegerValue(BidIncrementColumnName),
                        BuyNowPrice = reader.GetIntegerValueNullable(BuyNowPriceColumnName) ?? 0,
                        ReservePrice = reader.GetIntegerValueNullable(ReservePriceColumnName) ?? 0,

                        InventoryAuctionStatusCode = reader.GetStringValue(InventoryAuctionStatusCodeColumn),
                        InventoryAuctionStatusName = reader.GetStringValue(InventoryAuctionStatusNameColumn),

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
        /// GetList fetches and returns a list of InventoryAuctions (trimmed columns).
        /// </summary>
        public async Task<List<InventoryAuction>> GetList(InventoryAuction _)
        {
            var items = new List<InventoryAuction>();
            var parameters = new List<DbParameter>(); // none

            using (var reader = await base.ExecuteReader(parameters, GetListStoredProcedureName, CommandType.StoredProcedure))
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        var item = new InventoryAuction
                        {
                            InventoryAuctionId = reader.GetIntegerValue(InventoryAuctionIdColumnName),
                            InventoryAuctionStatusId = reader.GetIntegerValue(InventoryAuctionStatusIdColumnName),
                            InventoryId = reader.GetIntegerValue(InventoryIdColumnName),
                            AuctionId = reader.GetIntegerValue(AuctionIdColumnName),
                            BidIncrement = reader.GetIntegerValue(BidIncrementColumnName),
                            BuyNowPrice = reader.GetIntegerValueNullable(BuyNowPriceColumnName) ?? 0,
                            ReservePrice = reader.GetIntegerValueNullable(ReservePriceColumnName) ?? 0,

                            InventoryAuctionStatusCode = reader.GetStringValue(InventoryAuctionStatusCodeColumn),
                            InventoryAuctionStatusName = reader.GetStringValue(InventoryAuctionStatusNameColumn),

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
        /// Update updates an existing InventoryAuction and returns true if successful.
        /// (The SP refreshes the BidIncrement snapshot from Auction if only AuctionId is changed.)
        /// </summary>
        public async Task<bool> Update(InventoryAuction invAuc)
        {
            var parameters = new List<DbParameter>
            {
                base.GetParameter(InventoryAuctionIdParameterName, invAuc.InventoryAuctionId),

                base.GetParameter(InventoryAuctionStatusIdParameterName, invAuc.InventoryAuctionStatusId > 0 ? (object)invAuc.InventoryAuctionStatusId : DBNull.Value),
                base.GetParameter(InventoryIdParameterName,              invAuc.InventoryId              > 0 ? (object)invAuc.InventoryId              : DBNull.Value),
                base.GetParameter(AuctionIdParameterName,                invAuc.AuctionId                > 0 ? (object)invAuc.AuctionId                : DBNull.Value),


                base.GetParameter(BuyNowPriceParameterName,   invAuc.BuyNowPrice  >= 0 ? (object)invAuc.BuyNowPrice  : DBNull.Value),
                base.GetParameter(ReservePriceParameterName,  invAuc.ReservePrice >= 0 ? (object)invAuc.ReservePrice : DBNull.Value),

                base.GetParameter(ModifiedByIdParameterName,  invAuc.ModifiedById)
            };

            var rows = await base.ExecuteNonQuery(parameters, UpdateStoredProcedureName, CommandType.StoredProcedure);
            return rows > 0;
        }


        #endregion
    }
}
