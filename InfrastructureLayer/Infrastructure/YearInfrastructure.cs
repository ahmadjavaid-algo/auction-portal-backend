// AuctionPortal.InfrastructureLayer.Infrastructure/YearInfrastructure.cs
using AuctionPortal.Common.Infrastructure;
using AuctionPortal.InfrastructureLayer.Interfaces;
using AuctionPortal.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace AuctionPortal.InfrastructureLayer.Infrastructure
{
    public class YearInfrastructure : BaseInfrastructure, IYearInfrastructure
    {
        #region Constructor
        /// <summary>
        /// YearInfrastructure initializes class object.
        /// </summary>
        public YearInfrastructure(IConfiguration configuration, ILogger<YearInfrastructure> logger)
            : base(configuration, logger)
        {
        }
        #endregion

        #region Constants
        private const string AddStoredProcedureName = "[dbo].[sp_Year_Add]";
        private const string ActivateStoredProcedureName = "[dbo].[sp_Year_Activate]";
        private const string GetStoredProcedureName = "[dbo].[sp_Year_Get]";
        private const string GetListStoredProcedureName = "[dbo].[sp_Year_GetAll]";
        private const string UpdateStoredProcedureName = "[dbo].[sp_Year_Update]";

        private const string YearIdColumnName = "YearId";
        private const string ModelIdColumnName = "ModelId";
        private const string YearNameColumnName = "YearName";

        private const string YearIdParameterName = "@YearId";
        private const string ModelIdParameterName = "@ModelId";
        private const string YearNameParameterName = "@YearName";
        private const string CreatedByIdParameterName = "@CreatedById";
        private const string ModifiedByIdParameterName = "@ModifiedById";
        #endregion

        #region IYearInfrastructure Implementation

        /// <summary>
        /// Add adds new Year and returns generated YearId.
        /// </summary>
        public async Task<int> Add(Year Year)
        {
            var parameters = new List<DbParameter>
            {
                base.GetParameter(ModelIdParameterName, Year.ModelId),
                base.GetParameter(YearNameParameterName, Year.YearName),
                base.GetParameter(CreatedByIdParameterName, Year.CreatedById)
            };

            // sp_Year_Add INSERTs and SELECTs the inserted row.
            using (var reader = await base.ExecuteReader(parameters, AddStoredProcedureName, CommandType.StoredProcedure))
            {
                if (reader != null && reader.HasRows && reader.Read())
                {
                    Year.YearId = reader.GetIntegerValue(YearIdColumnName);
                    Year.ModelId = reader.GetIntegerValue(ModelIdColumnName);
                    Year.YearName = reader.GetStringValue(YearNameColumnName);

                    // Audit (optional to read on Add)
                    Year.CreatedById = reader.GetIntegerValueNullable(BaseInfrastructure.CreatedByIdColumnName);
                    Year.CreatedDate = reader.GetDateTimeValueNullable(BaseInfrastructure.CreatedDateColumnName);
                    Year.ModifiedById = reader.GetIntegerValueNullable(BaseInfrastructure.ModifiedByIdColumnName) ?? 0;
                    Year.ModifiedDate = reader.GetDateTimeValueNullable(BaseInfrastructure.ModifiedDateColumnName);
                    Year.Active = reader.GetBooleanValue(BaseInfrastructure.ActiveColumnName);
                }

                if (reader != null && !reader.IsClosed)
                    reader.Close();
            }

            return Year.YearId;
        }

        /// <summary>
        /// Activate activate/deactivate provided record and returns true if successful.
        /// </summary>
        public async Task<bool> Activate(Year Year)
        {
            var parameters = new List<DbParameter>
            {
                base.GetParameter(YearIdParameterName, Year.YearId),
                base.GetParameter(BaseInfrastructure.ActiveParameterName, Year.Active),
                base.GetParameter(ModifiedByIdParameterName, Year.ModifiedById)
            };

            var rows = await base.ExecuteNonQuery(parameters, ActivateStoredProcedureName, CommandType.StoredProcedure);
            return rows > 0;
        }

        /// <summary>
        /// Get fetches and returns a single Year.
        /// </summary>
        public async Task<Year> Get(Year Year)
        {
            Year item = null;

            var parameters = new List<DbParameter>
            {
                base.GetParameter(YearIdParameterName, Year.YearId)
            };

            using (var reader = await base.ExecuteReader(parameters, GetStoredProcedureName, CommandType.StoredProcedure))
            {
                if (reader != null && reader.HasRows && reader.Read())
                {
                    item = new Year
                    {
                        YearId = reader.GetIntegerValue(YearIdColumnName),
                        ModelId = reader.GetIntegerValue(ModelIdColumnName),
                        YearName = reader.GetStringValue(YearNameColumnName),

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
        /// GetList fetches and returns a list of Years (trimmed columns).
        /// </summary>
        public async Task<List<Year>> GetList(Year Year)
        {
            var items = new List<Year>();
            var parameters = new List<DbParameter>(); // none

            using (var reader = await base.ExecuteReader(parameters, GetListStoredProcedureName, CommandType.StoredProcedure))
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        var item = new Year
                        {
                            YearId = reader.GetIntegerValue(YearIdColumnName),
                            ModelId = reader.GetIntegerValue(ModelIdColumnName),
                            YearName = reader.GetStringValue(YearNameColumnName),
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
        /// Update updates an existing Year and returns true if successful.
        /// </summary>
        public async Task<bool> Update(Year year)
        {
            var parameters = new List<DbParameter>
            {
                base.GetParameter(YearIdParameterName, year.YearId),
                // pass NULL to keep existing YearName
                base.GetParameter(YearNameParameterName, (object?)year.YearName ?? DBNull.Value),
                // pass NULL to keep existing ModelId (example: treat 0 as "not provided")
                base.GetParameter(ModelIdParameterName, year.ModelId > 0 ? (object)year.ModelId : DBNull.Value),
                base.GetParameter(ModifiedByIdParameterName, year.ModifiedById)
            };

            var rows = await base.ExecuteNonQuery(parameters, UpdateStoredProcedureName, CommandType.StoredProcedure);
            return rows > 0;
        }


        #endregion
    }
}
