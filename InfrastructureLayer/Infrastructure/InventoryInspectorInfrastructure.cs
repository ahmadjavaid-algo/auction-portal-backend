// AuctionPortal.InfrastructureLayer.Infrastructure/InventoryInspectorInfrastructure.cs
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
    public class InventoryInspectorInfrastructure : BaseInfrastructure, IInventoryInspectorInfrastructure
    {
        #region Constructor
        public InventoryInspectorInfrastructure(
            IConfiguration configuration,
            ILogger<InventoryInspectorInfrastructure> logger)
            : base(configuration, logger)
        {
        }
        #endregion

        #region Constants
        private const string AddStoredProcedureName = "[dbo].[sp_InventoryInspector_Add]";
        private const string ActivateStoredProcedureName = "[dbo].[sp_InventoryInspector_Activate]";
        private const string GetStoredProcedureName = "[dbo].[sp_InventoryInspector_Get]";
        private const string GetListStoredProcedureName = "[dbo].[sp_InventoryInspector_GetAll]";
        private const string UpdateStoredProcedureName = "[dbo].[sp_InventoryInspector_Update]";

        private const string InventoryInspectorIdColumnName = "InventoryInspectorId";
        private const string AssignedToColumnName = "AssignedTo";
        private const string InventoryIdColumnName = "InventoryId";
        private const string InspectorNameColumnName = "InspectorName";

        private const string InventoryInspectorIdParameterName = "@InventoryInspectorId";
        private const string AssignedToParameterName = "@AssignedTo";
        private const string InventoryIdParameterName = "@InventoryId";
        private const string CreatedByIdParameterName = "@CreatedById";
        private const string ModifiedByIdParameterName = "@ModifiedById";
        #endregion

        #region IInventoryInspectorInfrastructure Implementation

        public async Task<int> Add(InventoryInspector inspector)
        {
            var parameters = new List<DbParameter>
            {
                base.GetParameter(AssignedToParameterName,
                    inspector.AssignedTo.HasValue && inspector.AssignedTo.Value > 0
                        ? (object)inspector.AssignedTo.Value
                        : DBNull.Value),
                base.GetParameter(InventoryIdParameterName, inspector.InventoryId),
                base.GetParameter(CreatedByIdParameterName, inspector.CreatedById)
            };

            using (var reader = await base.ExecuteReader(parameters, AddStoredProcedureName, CommandType.StoredProcedure))
            {
                if (reader != null && reader.HasRows && reader.Read())
                {
                    inspector.InventoryInspectorId = reader.GetIntegerValue(InventoryInspectorIdColumnName);
                    inspector.AssignedTo = reader.GetIntegerValueNullable(AssignedToColumnName);
                    inspector.InventoryId = reader.GetIntegerValue(InventoryIdColumnName);

                    // Audit
                    inspector.CreatedById = reader.GetIntegerValueNullable(BaseInfrastructure.CreatedByIdColumnName);
                    inspector.CreatedDate = reader.GetDateTimeValueNullable(BaseInfrastructure.CreatedDateColumnName);
                    inspector.ModifiedById = reader.GetIntegerValueNullable(BaseInfrastructure.ModifiedByIdColumnName) ?? 0;
                    inspector.ModifiedDate = reader.GetDateTimeValueNullable(BaseInfrastructure.ModifiedDateColumnName);
                    inspector.Active = reader.GetBooleanValue(BaseInfrastructure.ActiveColumnName);
                }

                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }
            }

            return inspector.InventoryInspectorId;
        }

        public async Task<bool> Activate(InventoryInspector inspector)
        {
            var parameters = new List<DbParameter>
            {
                base.GetParameter(InventoryInspectorIdParameterName, inspector.InventoryInspectorId),
                base.GetParameter(BaseInfrastructure.ActiveParameterName, inspector.Active),
                base.GetParameter(ModifiedByIdParameterName, inspector.ModifiedById)
            };

            var rows = await base.ExecuteNonQuery(parameters, ActivateStoredProcedureName, CommandType.StoredProcedure);
            return rows > 0;
        }

        public async Task<InventoryInspector> Get(InventoryInspector inspector)
        {
            InventoryInspector item = null;

            var parameters = new List<DbParameter>
            {
                base.GetParameter(InventoryInspectorIdParameterName, inspector.InventoryInspectorId)
            };

            using (var reader = await base.ExecuteReader(
                parameters,
                GetStoredProcedureName,
                CommandType.StoredProcedure))
            {
                if (reader != null && reader.HasRows && reader.Read())
                {
                    item = new InventoryInspector
                    {
                        InventoryInspectorId = reader.GetIntegerValue(InventoryInspectorIdColumnName),
                        AssignedTo = reader.GetIntegerValueNullable(AssignedToColumnName),
                        InventoryId = reader.GetIntegerValue(InventoryIdColumnName),

                        // string helper that exists in your project
                        InspectorName = reader.GetStringValue(InspectorNameColumnName),

                        CreatedById = reader.GetIntegerValueNullable(BaseInfrastructure.CreatedByIdColumnName),
                        CreatedDate = reader.GetDateTimeValueNullable(BaseInfrastructure.CreatedDateColumnName),
                        ModifiedById = reader.GetIntegerValueNullable(BaseInfrastructure.ModifiedByIdColumnName) ?? 0,
                        ModifiedDate = reader.GetDateTimeValueNullable(BaseInfrastructure.ModifiedDateColumnName),
                        Active = reader.GetBooleanValue(BaseInfrastructure.ActiveColumnName)
                    };
                }

                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }
            }

            return item;
        }

        public async Task<List<InventoryInspector>> GetList(InventoryInspector inspector)
        {
            var items = new List<InventoryInspector>();
            var parameters = new List<DbParameter>(); // none

            using (var reader = await base.ExecuteReader(
                parameters,
                GetListStoredProcedureName,
                CommandType.StoredProcedure))
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        var item = new InventoryInspector
                        {
                            InventoryInspectorId = reader.GetIntegerValue(InventoryInspectorIdColumnName),
                            AssignedTo = reader.GetIntegerValueNullable(AssignedToColumnName),
                            InventoryId = reader.GetIntegerValue(InventoryIdColumnName),
                            InspectorName = reader.GetStringValue(InspectorNameColumnName),
                            Active = reader.GetBooleanValue(BaseInfrastructure.ActiveColumnName),

                            CreatedById = reader.GetIntegerValueNullable(BaseInfrastructure.CreatedByIdColumnName),
                            CreatedDate = reader.GetDateTimeValueNullable(BaseInfrastructure.CreatedDateColumnName),
                            ModifiedById = reader.GetIntegerValueNullable(BaseInfrastructure.ModifiedByIdColumnName) ?? 0,
                            ModifiedDate = reader.GetDateTimeValueNullable(BaseInfrastructure.ModifiedDateColumnName)
                        };

                        items.Add(item);
                    }

                    if (!reader.IsClosed)
                    {
                        reader.Close();
                    }
                }
            }

            return items;
        }

        public async Task<bool> Update(InventoryInspector inspector)
        {
            var parameters = new List<DbParameter>
            {
                base.GetParameter(InventoryInspectorIdParameterName, inspector.InventoryInspectorId),

                base.GetParameter(AssignedToParameterName,
                    inspector.AssignedTo.HasValue && inspector.AssignedTo.Value > 0
                        ? (object)inspector.AssignedTo.Value
                        : DBNull.Value),
                base.GetParameter(InventoryIdParameterName,
                    inspector.InventoryId > 0 ? (object)inspector.InventoryId : DBNull.Value),

                base.GetParameter(ModifiedByIdParameterName, inspector.ModifiedById)
            };

            var rows = await base.ExecuteNonQuery(parameters, UpdateStoredProcedureName, CommandType.StoredProcedure);
            return rows > 0;
        }

        #endregion
    }
}
