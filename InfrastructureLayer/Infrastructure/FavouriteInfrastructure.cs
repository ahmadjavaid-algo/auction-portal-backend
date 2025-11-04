// AuctionPortal.InfrastructureLayer.Infrastructure/FavouriteInfrastructure.cs
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
    /// Infrastructure for bidder favourites (Favourite).
    /// </summary>
    public class FavouriteInfrastructure : BaseInfrastructure, IFavouriteInfrastructure
    {
        #region Constructor

        public FavouriteInfrastructure(IConfiguration configuration, ILogger<FavouriteInfrastructure> logger)
            : base(configuration, logger)
        {
        }

        #endregion

        #region Constants

        private const string AddStoredProcedureName = "[dbo].[sp_BidderInventoryAuctionFavorite_Add]";
        private const string ActivateStoredProcedureName = "[dbo].[sp_BidderInventoryAuctionFavorite_Activate]";
        private const string GetStoredProcedureName = "[dbo].[sp_BidderInventoryAuctionFavorite_Get]";
        private const string GetListStoredProcedureName = "[dbo].[sp_BidderInventoryAuctionFavorite_GetAll]";
        private const string UpdateStoredProcedureName = "[dbo].[sp_BidderInventoryAuctionFavorite_Update]";

        // Column names returned by the SPs
        private const string FavouriteIdColumnName = "BidderInventoryAuctionFavoriteId";
        private const string UserIdColumnName = "UserId";
        private const string InventoryAuctionIdColumnName = "InventoryAuctionId";

        // Parameter names (match SP signatures)
        private const string FavouriteIdParameterName = "@BidderInventoryAuctionFavoriteId";
        private const string UserIdParameterName = "@UserId";
        private const string InventoryAuctionIdParameterName = "@InventoryAuctionId";
        private const string CreatedByIdParameterName = "@CreatedById";
        private const string ModifiedByIdParameterName = "@ModifiedById";

        #endregion

        #region IFavouriteInfrastructure Implementation

        /// <summary>
        /// Add adds a new favourite (UserId + InventoryAuctionId) and returns generated FavouriteId.
        /// </summary>
        public async Task<int> Add(Favourite fav)
        {
            var parameters = new List<DbParameter>
            {
                base.GetParameter(UserIdParameterName,              fav.UserId),
                base.GetParameter(InventoryAuctionIdParameterName,  fav.InventoryAuctionId),
                base.GetParameter(CreatedByIdParameterName,         fav.CreatedById)
            };

            using (var reader = await base.ExecuteReader(parameters, AddStoredProcedureName, CommandType.StoredProcedure))
            {
                if (reader != null && reader.HasRows && reader.Read())
                {
                    fav.BidderInventoryAuctionFavoriteId = reader.GetIntegerValue(FavouriteIdColumnName);
                    fav.UserId = reader.GetIntegerValue(UserIdColumnName);
                    fav.InventoryAuctionId = reader.GetIntegerValue(InventoryAuctionIdColumnName);

                    fav.CreatedById = reader.GetIntegerValueNullable(BaseInfrastructure.CreatedByIdColumnName);
                    fav.CreatedDate = reader.GetDateTimeValueNullable(BaseInfrastructure.CreatedDateColumnName);
                    fav.ModifiedById = reader.GetIntegerValueNullable(BaseInfrastructure.ModifiedByIdColumnName) ?? 0;
                    fav.ModifiedDate = reader.GetDateTimeValueNullable(BaseInfrastructure.ModifiedDateColumnName);
                    fav.Active = reader.GetBooleanValue(BaseInfrastructure.ActiveColumnName);
                }

                if (reader != null && !reader.IsClosed)
                    reader.Close();
            }

            return fav.BidderInventoryAuctionFavoriteId;
        }

        /// <summary>
        /// Activate activate/deactivate provided favourite and returns true if successful.
        /// </summary>
        public async Task<bool> Activate(Favourite fav)
        {
            var parameters = new List<DbParameter>
            {
                base.GetParameter(FavouriteIdParameterName,                 fav.BidderInventoryAuctionFavoriteId),
                base.GetParameter(BaseInfrastructure.ActiveParameterName,   fav.Active),
                base.GetParameter(ModifiedByIdParameterName,                fav.ModifiedById)
            };

            var rows = await base.ExecuteNonQuery(parameters, ActivateStoredProcedureName, CommandType.StoredProcedure);
            return rows > 0;
        }

        /// <summary>
        /// Get fetches and returns a single favourite.
        /// </summary>
        public async Task<Favourite> Get(Favourite fav)
        {
            Favourite item = null;

            var parameters = new List<DbParameter>
            {
                base.GetParameter(FavouriteIdParameterName, fav.BidderInventoryAuctionFavoriteId)
            };

            using (var reader = await base.ExecuteReader(parameters, GetStoredProcedureName, CommandType.StoredProcedure))
            {
                if (reader != null && reader.HasRows && reader.Read())
                {
                    item = new Favourite
                    {
                        BidderInventoryAuctionFavoriteId = reader.GetIntegerValue(FavouriteIdColumnName),
                        UserId = reader.GetIntegerValue(UserIdColumnName),
                        InventoryAuctionId = reader.GetIntegerValue(InventoryAuctionIdColumnName),

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
        /// GetList fetches and returns a list of favourites.
        /// </summary>
        public async Task<List<Favourite>> GetList(Favourite _)
        {
            var items = new List<Favourite>();
            var parameters = new List<DbParameter>(); // none

            using (var reader = await base.ExecuteReader(parameters, GetListStoredProcedureName, CommandType.StoredProcedure))
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        var item = new Favourite
                        {
                            BidderInventoryAuctionFavoriteId = reader.GetIntegerValue(FavouriteIdColumnName),
                            UserId = reader.GetIntegerValue(UserIdColumnName),
                            InventoryAuctionId = reader.GetIntegerValue(InventoryAuctionIdColumnName),

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
        /// Update updates an existing favourite and returns true if successful.
        /// </summary>
        public async Task<bool> Update(Favourite fav)
        {
            var parameters = new List<DbParameter>
            {
                base.GetParameter(FavouriteIdParameterName, fav.BidderInventoryAuctionFavoriteId),

                base.GetParameter(UserIdParameterName,
                    fav.UserId > 0 ? (object)fav.UserId : DBNull.Value),
                base.GetParameter(InventoryAuctionIdParameterName,
                    fav.InventoryAuctionId > 0 ? (object)fav.InventoryAuctionId : DBNull.Value),

                base.GetParameter(ModifiedByIdParameterName, fav.ModifiedById)
            };

            var rows = await base.ExecuteNonQuery(parameters, UpdateStoredProcedureName, CommandType.StoredProcedure);
            return rows > 0;
        }

        #endregion
    }
}
