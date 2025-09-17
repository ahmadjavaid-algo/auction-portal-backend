// AuctionPortal.InfrastructureLayer.Infrastructure/MakeInfrastructure.cs
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
    public class MakeInfrastructure : BaseInfrastructure, IMakeInfrastructure
    {
        #region Constructor
        /// <summary>
        /// MakeInfrastructure initializes class object.
        /// </summary>
        public MakeInfrastructure(IConfiguration configuration, ILogger<MakeInfrastructure> logger)
            : base(configuration, logger)
        {
        }
        #endregion

        #region Constants
        private const string AddStoredProcedureName = "[dbo].[sp_Make_Add]";
        private const string ActivateStoredProcedureName = "[dbo].[sp_Make_Activate]";
        private const string GetStoredProcedureName = "[dbo].[sp_Make_Get]";
        private const string GetListStoredProcedureName = "[dbo].[sp_Make_GetAll]";
        private const string UpdateStoredProcedureName = "[dbo].[sp_Make_Update]";

        private const string MakeIdColumnName = "MakeId";
        private const string MakeNameColumnName = "MakeName";

        private const string MakeIdParameterName = "@MakeId";
        private const string MakeNameParameterName = "@MakeName";
        private const string CreatedByIdParameterName = "@CreatedById";
        private const string ModifiedByIdParameterName = "@ModifiedById";
        #endregion

        #region IMakeInfrastructure Implementation

        /// <summary>
        /// Add adds new Make and returns generated MakeId.
        /// </summary>
        public async Task<int> Add(Make make)
        {
            var parameters = new List<DbParameter>
            {
                base.GetParameter(MakeNameParameterName, make.MakeName),
                base.GetParameter(CreatedByIdParameterName, make.CreatedById)
            };

            // sp_Make_Add INSERTs and SELECTs the inserted row.
            using (var reader = await base.ExecuteReader(parameters, AddStoredProcedureName, CommandType.StoredProcedure))
            {
                if (reader != null && reader.HasRows && reader.Read())
                {
                    make.MakeId = reader.GetIntegerValue(MakeIdColumnName);
                    make.MakeName = reader.GetStringValue(MakeNameColumnName);

                    // Audit (optional to read on Add)
                    make.CreatedById = reader.GetIntegerValueNullable(BaseInfrastructure.CreatedByIdColumnName);
                    make.CreatedDate = reader.GetDateTimeValueNullable(BaseInfrastructure.CreatedDateColumnName);
                    make.ModifiedById = reader.GetIntegerValueNullable(BaseInfrastructure.ModifiedByIdColumnName) ?? 0;
                    make.ModifiedDate = reader.GetDateTimeValueNullable(BaseInfrastructure.ModifiedDateColumnName);
                    make.Active = reader.GetBooleanValue(BaseInfrastructure.ActiveColumnName);
                }

                if (reader != null && !reader.IsClosed)
                    reader.Close();
            }

            return make.MakeId;
        }

        /// <summary>
        /// Activate activate/deactivate provided record and returns true if successful.
        /// </summary>
        public async Task<bool> Activate(Make make)
        {
            var parameters = new List<DbParameter>
            {
                base.GetParameter(MakeIdParameterName, make.MakeId),
                base.GetParameter(BaseInfrastructure.ActiveParameterName, make.Active),
                base.GetParameter(ModifiedByIdParameterName, make.ModifiedById)
            };

            var rows = await base.ExecuteNonQuery(parameters, ActivateStoredProcedureName, CommandType.StoredProcedure);
            return rows > 0;
        }

        /// <summary>
        /// Get fetches and returns a single Make.
        /// </summary>
        public async Task<Make> Get(Make make)
        {
            Make item = null;

            var parameters = new List<DbParameter>
            {
                base.GetParameter(MakeIdParameterName, make.MakeId)
            };

            using (var reader = await base.ExecuteReader(parameters, GetStoredProcedureName, CommandType.StoredProcedure))
            {
                if (reader != null && reader.HasRows && reader.Read())
                {
                    item = new Make
                    {
                        MakeId = reader.GetIntegerValue(MakeIdColumnName),
                        MakeName = reader.GetStringValue(MakeNameColumnName),

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
        /// GetList fetches and returns a list of Makes (trimmed columns).
        /// </summary>
        public async Task<List<Make>> GetList(Make make)
        {
            var items = new List<Make>();
            var parameters = new List<DbParameter>(); // none

            using (var reader = await base.ExecuteReader(parameters, GetListStoredProcedureName, CommandType.StoredProcedure))
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        var item = new Make
                        {
                            MakeId = reader.GetIntegerValue(MakeIdColumnName),
                            MakeName = reader.GetStringValue(MakeNameColumnName),
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
        /// Update updates an existing Make and returns true if successful.
        /// </summary>
        public async Task<bool> Update(Make make)
        {
            var parameters = new List<DbParameter>
            {
                base.GetParameter(MakeIdParameterName, make.MakeId),
                base.GetParameter(MakeNameParameterName, make.MakeName),
                base.GetParameter(ModifiedByIdParameterName, make.ModifiedById)
            };

            var rows = await base.ExecuteNonQuery(parameters, UpdateStoredProcedureName, CommandType.StoredProcedure);
            return rows > 0;
        }

        #endregion
    }
}
