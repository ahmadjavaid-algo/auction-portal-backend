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
    public class InspectionInfrastructure : BaseInfrastructure, IInspectionInfrastructure
    {
        #region Constructor
        public InspectionInfrastructure(IConfiguration configuration, ILogger<InspectionInfrastructure> logger)
            : base(configuration, logger) { }
        #endregion

        #region Constants

        private const string AddStoredProcedureName = "[dbo].[sp_Inspection_Add]";
        private const string UpdateStoredProcedureName = "[dbo].[sp_Inspection_Update]";
        private const string ActivateStoredProcedureName = "[dbo].[sp_Inspection_Activate]";
        private const string GetStoredProcedureName = "[dbo].[sp_Inspection_Get]";
        private const string GetListStoredProcedureName = "[dbo].[sp_Inspection_GetAll]";
        private const string GetByInventoryStoredProcedureName = "[dbo].[sp_Inspection_GetByInventory]";

        // Column names
        private const string InspectionIdColumnName = "InspectionId";
        private const string InspectionTypeIdColumnName = "InspectionTypeId";
        private const string InspectionCheckpointIdColumnName = "InspectionCheckpointId";
        private const string InventoryIdColumnName = "InventoryId";
        private const string ResultColumnName = "Result";

        private const string InspectionTypeNameColumnName = "InspectionTypeName";
        private const string InspectionCheckpointNameColumn = "InspectionCheckpointName";
        private const string InputTypeColumnName = "InputType";

        private const string ProductIdColumnName = "ProductId";
        private const string ProductDisplayNameColumnName = "ProductDisplayName";
        private const string ProductJSONColumnName = "ProductJSON";
        private const string InventoryDescriptionColumnName = "InventoryDescription";

        // Parameter names
        private const string InspectionIdParameterName = "@InspectionId";
        private const string InspectionTypeIdParameterName = "@InspectionTypeId";
        private const string InspectionCheckpointIdParamName = "@InspectionCheckpointId";
        private const string InventoryIdParameterName = "@InventoryId";
        private const string ResultParameterName = "@Result";
        private const string CreatedByIdParameterName = "@CreatedById";
        private const string ModifiedByIdParameterName = "@ModifiedById";

        #endregion

        #region IInspectionInfrastructure Implementation

        public async Task<int> Add(Inspection inspection)
        {
            var parameters = new List<DbParameter>
            {
                base.GetParameter(InspectionTypeIdParameterName,      inspection.InspectionTypeId),
                base.GetParameter(InspectionCheckpointIdParamName,    inspection.InspectionCheckpointId),
                base.GetParameter(InventoryIdParameterName,           inspection.InventoryId),
                base.GetParameter(ResultParameterName,                inspection.Result ?? (object)DBNull.Value),
                base.GetParameter(CreatedByIdParameterName,           inspection.CreatedById)
            };

            using (var reader = await base.ExecuteReader(parameters, AddStoredProcedureName, CommandType.StoredProcedure))
            {
                if (reader != null && reader.HasRows && reader.Read())
                {
                    inspection.InspectionId = reader.GetIntegerValue(InspectionIdColumnName);
                    inspection.InspectionTypeId = reader.GetIntegerValue(InspectionTypeIdColumnName);
                    inspection.InspectionCheckpointId = reader.GetIntegerValue(InspectionCheckpointIdColumnName);
                    inspection.InventoryId = reader.GetIntegerValue(InventoryIdColumnName);
                    inspection.Result = reader.GetStringValue(ResultColumnName);

                    // Audit
                    inspection.CreatedById = reader.GetIntegerValueNullable(BaseInfrastructure.CreatedByIdColumnName);
                    inspection.CreatedDate = reader.GetDateTimeValueNullable(BaseInfrastructure.CreatedDateColumnName);
                    inspection.ModifiedById = reader.GetIntegerValueNullable(BaseInfrastructure.ModifiedByIdColumnName) ?? 0;
                    inspection.ModifiedDate = reader.GetDateTimeValueNullable(BaseInfrastructure.ModifiedDateColumnName);
                    inspection.Active = reader.GetBooleanValue(BaseInfrastructure.ActiveColumnName);
                }

                if (reader != null && !reader.IsClosed)
                    reader.Close();
            }

            return inspection.InspectionId;
        }

        public async Task<bool> Update(Inspection inspection)
        {
            var parameters = new List<DbParameter>
            {
                base.GetParameter(InspectionIdParameterName,          inspection.InspectionId),
                base.GetParameter(InspectionTypeIdParameterName,      inspection.InspectionTypeId > 0 ? (object)inspection.InspectionTypeId : DBNull.Value),
                base.GetParameter(InspectionCheckpointIdParamName,    inspection.InspectionCheckpointId > 0 ? (object)inspection.InspectionCheckpointId : DBNull.Value),
                base.GetParameter(InventoryIdParameterName,           inspection.InventoryId > 0 ? (object)inspection.InventoryId : DBNull.Value),
                base.GetParameter(ResultParameterName,                (object?)inspection.Result ?? DBNull.Value),
                base.GetParameter(ModifiedByIdParameterName,          inspection.ModifiedById)
            };

            var rows = await base.ExecuteNonQuery(parameters, UpdateStoredProcedureName, CommandType.StoredProcedure);
            return rows > 0;
        }

        public async Task<bool> Activate(Inspection inspection)
        {
            var parameters = new List<DbParameter>
            {
                base.GetParameter(InspectionIdParameterName, inspection.InspectionId),
                base.GetParameter(BaseInfrastructure.ActiveParameterName, inspection.Active),
                base.GetParameter(ModifiedByIdParameterName, inspection.ModifiedById)
            };

            var rows = await base.ExecuteNonQuery(parameters, ActivateStoredProcedureName, CommandType.StoredProcedure);
            return rows > 0;
        }

        public async Task<Inspection> Get(Inspection inspection)
        {
            Inspection item = null;

            var parameters = new List<DbParameter>
            {
                base.GetParameter(InspectionIdParameterName, inspection.InspectionId)
            };

            using (var reader = await base.ExecuteReader(parameters, GetStoredProcedureName, CommandType.StoredProcedure))
            {
                if (reader != null && reader.HasRows && reader.Read())
                {
                    item = new Inspection
                    {
                        InspectionId = reader.GetIntegerValue(InspectionIdColumnName),
                        InspectionTypeId = reader.GetIntegerValue(InspectionTypeIdColumnName),
                        InspectionCheckpointId = reader.GetIntegerValue(InspectionCheckpointIdColumnName),
                        InventoryId = reader.GetIntegerValue(InventoryIdColumnName),
                        Result = reader.GetStringValue(ResultColumnName),

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

        public async Task<List<Inspection>> GetList(Inspection inspection)
        {
            var items = new List<Inspection>();
            var parameters = new List<DbParameter>(); // none

            using (var reader = await base.ExecuteReader(parameters, GetListStoredProcedureName, CommandType.StoredProcedure))
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        var item = new Inspection
                        {
                            InspectionId = reader.GetIntegerValue(InspectionIdColumnName),

                            InspectionTypeId = reader.GetIntegerValue(InspectionTypeIdColumnName),
                            InspectionTypeName = reader.GetStringValue(InspectionTypeNameColumnName),

                            InspectionCheckpointId = reader.GetIntegerValue(InspectionCheckpointIdColumnName),
                            InspectionCheckpointName = reader.GetStringValue(InspectionCheckpointNameColumn),
                            InputType = reader.GetStringValue(InputTypeColumnName),

                            InventoryId = reader.GetIntegerValue(InventoryIdColumnName),
                            ProductId = reader.GetIntegerValue(ProductIdColumnName),
                            ProductDisplayName = reader.GetStringValue(ProductDisplayNameColumnName),
                            ProductJSON = reader.GetStringValue(ProductJSONColumnName),
                            InventoryDescription = reader.GetStringValue(InventoryDescriptionColumnName),

                            Result = reader.GetStringValue(ResultColumnName),
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

        public async Task<List<Inspection>> GetByInventory(Inspection inspection)
        {
            var items = new List<Inspection>();

            var parameters = new List<DbParameter>
    {
        base.GetParameter(InventoryIdParameterName, inspection.InventoryId)
    };

            using (var reader = await base.ExecuteReader(
                       parameters,
                       GetByInventoryStoredProcedureName,
                       CommandType.StoredProcedure))
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        var item = new Inspection
                        {
                            
                            InspectionId = reader.GetIntegerValueNullable(InspectionIdColumnName) ?? 0,

                            InspectionTypeId = reader.GetIntegerValue(InspectionTypeIdColumnName),
                            InspectionTypeName = reader.GetStringValue(InspectionTypeNameColumnName),

                            InspectionCheckpointId = reader.GetIntegerValue(InspectionCheckpointIdColumnName),
                            InspectionCheckpointName = reader.GetStringValue(InspectionCheckpointNameColumn),
                            InputType = reader.GetStringValue(InputTypeColumnName),

                            InventoryId = reader.GetIntegerValue(InventoryIdColumnName),
                            ProductId = reader.GetIntegerValue(ProductIdColumnName),
                            ProductDisplayName = reader.GetStringValue(ProductDisplayNameColumnName),
                            ProductJSON = reader.GetStringValue(ProductJSONColumnName),
                            InventoryDescription = reader.GetStringValue(InventoryDescriptionColumnName),

                            
                            Result = reader.GetStringValue(ResultColumnName),

                           
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


        #endregion
    }
}
