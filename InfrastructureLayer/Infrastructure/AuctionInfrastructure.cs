// AuctionPortal.InfrastructureLayer.Infrastructure/AuctionInfrastructure.cs
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
    public class AuctionInfrastructure : BaseInfrastructure, IAuctionInfrastructure
    {
        #region Constructor
        /// <summary>
        /// AuctionInfrastructure initializes class object.
        /// </summary>
        public AuctionInfrastructure(IConfiguration configuration, ILogger<AuctionInfrastructure> logger)
            : base(configuration, logger)
        {
        }
        #endregion

        #region Constants
        private const string AddStoredProcedureName = "[dbo].[sp_Auction_Add]";
        private const string ActivateStoredProcedureName = "[dbo].[sp_Auction_Activate]";
        private const string GetStoredProcedureName = "[dbo].[sp_Auction_Get]";
        private const string GetListStoredProcedureName = "[dbo].[sp_Auction_GetAll]";
        private const string UpdateStoredProcedureName = "[dbo].[sp_Auction_Update]";

        // Column names (as selected by the SPs)
        private const string AuctionIdColumnName = "AuctionId";
        private const string AuctionStatusIdColumnName = "AuctionStatusId";
        private const string AuctionNameColumnName = "AuctionName";
        private const string StartDateTimeColumnName = "StartDateTime";
        private const string EndDateTimeColumnName = "EndDateTime";
        private const string BidIncrementColumnName = "BidIncrement";
        private const string AuctionStatusCodeColumnName = "AuctionStatusCode";  
        private const string AuctionStatusNameColumnName = "AuctionStatusName"; 

        // Parameter names (match SP signatures)
        private const string AuctionIdParameterName = "@AuctionId";
        private const string AuctionStatusIdParameterName = "@AuctionStatusId";
        private const string AuctionNameParameterName = "@AuctionName";
        private const string StartDateTimeParameterName = "@StartDateTime";
        private const string EndDateTimeParameterName = "@EndDateTime";
        private const string BidIncrementParameterName = "@BidIncrement";
        private const string CreatedByIdParameterName = "@CreatedById";
        private const string ModifiedByIdParameterName = "@ModifiedById";
        #endregion

        #region IAuctionInfrastructure Implementation

        /// <summary>
        /// Add adds new Auction and returns generated AuctionId.
        /// </summary>
        public async Task<int> Add(Auction auction)
        {
            var parameters = new List<DbParameter>
            {
                base.GetParameter(AuctionStatusIdParameterName, auction.AuctionStatusId),
                base.GetParameter(AuctionNameParameterName,     auction.AuctionName),
                base.GetParameter(StartDateTimeParameterName,   auction.StartDateTime),
                base.GetParameter(EndDateTimeParameterName,     auction.EndDateTime),
                base.GetParameter(BidIncrementParameterName,    auction.BidIncrement),
                base.GetParameter(CreatedByIdParameterName,     auction.CreatedById)
            };

            // sp_Auction_Add INSERTs and SELECTs the inserted row.
            using (var reader = await base.ExecuteReader(parameters, AddStoredProcedureName, CommandType.StoredProcedure))
            {
                if (reader != null && reader.HasRows && reader.Read())
                {
                    auction.AuctionId = reader.GetIntegerValue(AuctionIdColumnName);
                    auction.AuctionStatusId = reader.GetIntegerValue(AuctionStatusIdColumnName);
                    auction.AuctionName = reader.GetStringValue(AuctionNameColumnName);
                    auction.StartDateTime = reader.GetDateTimeValue(StartDateTimeColumnName);
                    auction.EndDateTime = reader.GetDateTimeValue(EndDateTimeColumnName);
                    auction.BidIncrement = reader.GetIntegerValue(BidIncrementColumnName);

                    // Optional joined fields (safe reads)
                    auction.AuctionStatusCode = reader.GetStringValue(AuctionStatusCodeColumnName);
                    auction.AuctionStatusName = reader.GetStringValue(AuctionStatusNameColumnName);

                    // Audit
                    auction.CreatedById = reader.GetIntegerValueNullable(BaseInfrastructure.CreatedByIdColumnName);
                    auction.CreatedDate = reader.GetDateTimeValueNullable(BaseInfrastructure.CreatedDateColumnName);
                    auction.ModifiedById = reader.GetIntegerValueNullable(BaseInfrastructure.ModifiedByIdColumnName) ?? 0;
                    auction.ModifiedDate = reader.GetDateTimeValueNullable(BaseInfrastructure.ModifiedDateColumnName);
                    auction.Active = reader.GetBooleanValue(BaseInfrastructure.ActiveColumnName);
                }

                if (reader != null && !reader.IsClosed)
                    reader.Close();
            }

            return auction.AuctionId;
        }

        /// <summary>
        /// Activate activate/deactivate provided record and returns true if successful.
        /// </summary>
        public async Task<bool> Activate(Auction auction)
        {
            var parameters = new List<DbParameter>
            {
                base.GetParameter(AuctionIdParameterName, auction.AuctionId),
                base.GetParameter(BaseInfrastructure.ActiveParameterName, auction.Active),
                base.GetParameter(ModifiedByIdParameterName, auction.ModifiedById)
            };

            var rows = await base.ExecuteNonQuery(parameters, ActivateStoredProcedureName, CommandType.StoredProcedure);
            return rows > 0;
        }

        /// <summary>
        /// Get fetches and returns a single Auction.
        /// </summary>
        public async Task<Auction> Get(Auction auction)
        {
            Auction item = null;

            var parameters = new List<DbParameter>
            {
                base.GetParameter(AuctionIdParameterName, auction.AuctionId)
            };

            using (var reader = await base.ExecuteReader(parameters, GetStoredProcedureName, CommandType.StoredProcedure))
            {
                if (reader != null && reader.HasRows && reader.Read())
                {
                    item = new Auction
                    {
                        AuctionId = reader.GetIntegerValue(AuctionIdColumnName),
                        AuctionStatusId = reader.GetIntegerValue(AuctionStatusIdColumnName),
                        AuctionName = reader.GetStringValue(AuctionNameColumnName),
                        StartDateTime = reader.GetDateTimeValue(StartDateTimeColumnName),
                        EndDateTime = reader.GetDateTimeValue(EndDateTimeColumnName),
                        BidIncrement = reader.GetIntegerValue(BidIncrementColumnName),

                        
                        AuctionStatusCode = reader.GetStringValue(AuctionStatusCodeColumnName),
                        AuctionStatusName = reader.GetStringValue(AuctionStatusNameColumnName),

                     
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
        /// GetList fetches and returns a list of Auctions (trimmed columns).
        /// </summary>
        public async Task<List<Auction>> GetList(Auction auction)
        {
            var items = new List<Auction>();
            var parameters = new List<DbParameter>(); // none

            using (var reader = await base.ExecuteReader(parameters, GetListStoredProcedureName, CommandType.StoredProcedure))
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        var item = new Auction
                        {
                            AuctionId = reader.GetIntegerValue(AuctionIdColumnName),
                            AuctionStatusId = reader.GetIntegerValue(AuctionStatusIdColumnName),
                            AuctionName = reader.GetStringValue(AuctionNameColumnName),
                            StartDateTime = reader.GetDateTimeValue(StartDateTimeColumnName),
                            EndDateTime = reader.GetDateTimeValue(EndDateTimeColumnName),
                            BidIncrement = reader.GetIntegerValue(BidIncrementColumnName),

                            // Optional joined fields
                            AuctionStatusCode = reader.GetStringValue(AuctionStatusCodeColumnName),
                            AuctionStatusName = reader.GetStringValue(AuctionStatusNameColumnName),

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
        /// Update updates an existing Auction and returns true if successful.
        /// </summary>
        public async Task<bool> Update(Auction auction)
        {
            var parameters = new List<DbParameter>
            {
                base.GetParameter(AuctionIdParameterName, auction.AuctionId),

                base.GetParameter(AuctionNameParameterName,  (object?)auction.AuctionName ?? DBNull.Value),

                base.GetParameter(AuctionStatusIdParameterName, auction.AuctionStatusId > 0 ? (object)auction.AuctionStatusId : DBNull.Value),
                base.GetParameter(StartDateTimeParameterName, auction.StartDateTime != default ? (object)auction.StartDateTime : DBNull.Value),
                base.GetParameter(EndDateTimeParameterName,   auction.EndDateTime   != default ? (object)auction.EndDateTime   : DBNull.Value),
                base.GetParameter(BidIncrementParameterName,  auction.BidIncrement  >= 0      ? (object)auction.BidIncrement  : DBNull.Value),

                base.GetParameter(ModifiedByIdParameterName, auction.ModifiedById)
            };

            var rows = await base.ExecuteNonQuery(parameters, UpdateStoredProcedureName, CommandType.StoredProcedure);
            return rows > 0;
        }

        #endregion
    }
}
