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
    public class InspectionCheckpointInfrastructure : BaseInfrastructure, IInspectionCheckpointInfrastructure
    {
        #region Constructor
        public InspectionCheckpointInfrastructure(IConfiguration configuration, ILogger<InspectionCheckpointInfrastructure> logger)
            : base(configuration, logger) { }
        #endregion

        #region Constants

        private const string AddStoredProcedureName = "[dbo].[sp_InspectionCheckpoint_Add]";
        private const string ActivateStoredProcedureName = "[dbo].[sp_InspectionCheckpoint_Activate]";
        private const string GetStoredProcedureName = "[dbo].[sp_InspectionCheckpoint_Get]";
        private const string GetListStoredProcedureName = "[dbo].[sp_InspectionCheckpoint_GetAll]";
        private const string UpdateStoredProcedureName = "[dbo].[sp_InspectionCheckpoint_Update]";

        private const string InspectionCheckpointIdColumnName = "InspectionCheckpointId";
        private const string InspectionTypeIdColumnName = "InspectionTypeId";
        private const string InspectionCheckpointNameColumnName = "InspectionCheckpointName";
        private const string InputTypeColumnName = "InputType";
        private const string InspectionTypeNameColumnName = "InspectionTypeName";

        private const string InspectionCheckpointIdParameterName = "@InspectionCheckpointId";
        private const string InspectionTypeIdParameterName = "@InspectionTypeId";
        private const string InspectionCheckpointNameParameterName = "@InspectionCheckpointName";
        private const string InputTypeParameterName = "@InputType";
        private const string CreatedByIdParameterName = "@CreatedById";
        private const string ModifiedByIdParameterName = "@ModifiedById";

        #endregion

        #region IInspectionCheckpointInfrastructure Implementation

        public async Task<int> Add(InspectionCheckpoint checkpoint)
        {
            var parameters = new List<DbParameter>
            {
                base.GetParameter(InspectionTypeIdParameterName,         checkpoint.InspectionTypeId),
                base.GetParameter(InspectionCheckpointNameParameterName, checkpoint.InspectionCheckpointName),
                base.GetParameter(InputTypeParameterName,                checkpoint.InputType),
                base.GetParameter(CreatedByIdParameterName,              checkpoint.CreatedById)
            };

            using (var reader = await base.ExecuteReader(parameters, AddStoredProcedureName, CommandType.StoredProcedure))
            {
                if (reader != null && reader.HasRows && reader.Read())
                {
                    checkpoint.InspectionCheckpointId = reader.GetIntegerValue(InspectionCheckpointIdColumnName);
                    checkpoint.InspectionTypeId = reader.GetIntegerValue(InspectionTypeIdColumnName);
                    checkpoint.InspectionCheckpointName = reader.GetStringValue(InspectionCheckpointNameColumnName);
                    checkpoint.InputType = reader.GetStringValue(InputTypeColumnName);

                    checkpoint.CreatedById = reader.GetIntegerValueNullable(BaseInfrastructure.CreatedByIdColumnName);
                    checkpoint.CreatedDate = reader.GetDateTimeValueNullable(BaseInfrastructure.CreatedDateColumnName);
                    checkpoint.ModifiedById = reader.GetIntegerValueNullable(BaseInfrastructure.ModifiedByIdColumnName) ?? 0;
                    checkpoint.ModifiedDate = reader.GetDateTimeValueNullable(BaseInfrastructure.ModifiedDateColumnName);
                    checkpoint.Active = reader.GetBooleanValue(BaseInfrastructure.ActiveColumnName);
                }

                if (reader != null && !reader.IsClosed) reader.Close();
            }

            return checkpoint.InspectionCheckpointId;
        }

        public async Task<bool> Activate(InspectionCheckpoint checkpoint)
        {
            var parameters = new List<DbParameter>
            {
                base.GetParameter(InspectionCheckpointIdParameterName, checkpoint.InspectionCheckpointId),
                base.GetParameter(BaseInfrastructure.ActiveParameterName, checkpoint.Active),
                base.GetParameter(ModifiedByIdParameterName, checkpoint.ModifiedById)
            };

            var rows = await base.ExecuteNonQuery(parameters, ActivateStoredProcedureName, CommandType.StoredProcedure);
            return rows > 0;
        }

        public async Task<InspectionCheckpoint> Get(InspectionCheckpoint checkpoint)
        {
            InspectionCheckpoint item = null;

            var parameters = new List<DbParameter>
            {
                base.GetParameter(InspectionCheckpointIdParameterName, checkpoint.InspectionCheckpointId)
            };

            using (var reader = await base.ExecuteReader(parameters, GetStoredProcedureName, CommandType.StoredProcedure))
            {
                if (reader != null && reader.HasRows && reader.Read())
                {
                    item = new InspectionCheckpoint
                    {
                        InspectionCheckpointId = reader.GetIntegerValue(InspectionCheckpointIdColumnName),
                        InspectionTypeId = reader.GetIntegerValue(InspectionTypeIdColumnName),
                        InspectionCheckpointName = reader.GetStringValue(InspectionCheckpointNameColumnName),
                        InputType = reader.GetStringValue(InputTypeColumnName),

                        CreatedById = reader.GetIntegerValueNullable(BaseInfrastructure.CreatedByIdColumnName),
                        CreatedDate = reader.GetDateTimeValueNullable(BaseInfrastructure.CreatedDateColumnName),
                        ModifiedById = reader.GetIntegerValueNullable(BaseInfrastructure.ModifiedByIdColumnName) ?? 0,
                        ModifiedDate = reader.GetDateTimeValueNullable(BaseInfrastructure.ModifiedDateColumnName),
                        Active = reader.GetBooleanValue(BaseInfrastructure.ActiveColumnName)
                    };
                }

                if (reader != null && !reader.IsClosed) reader.Close();
            }

            return item;
        }

        public async Task<List<InspectionCheckpoint>> GetList(InspectionCheckpoint checkpoint)
        {
            var items = new List<InspectionCheckpoint>();
            var parameters = new List<DbParameter>(); // no filters

            using (var reader = await base.ExecuteReader(parameters, GetListStoredProcedureName, CommandType.StoredProcedure))
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        var item = new InspectionCheckpoint
                        {
                            InspectionCheckpointId = reader.GetIntegerValue(InspectionCheckpointIdColumnName),
                            InspectionTypeId = reader.GetIntegerValue(InspectionTypeIdColumnName),
                            InspectionCheckpointName = reader.GetStringValue(InspectionCheckpointNameColumnName),
                            InputType = reader.GetStringValue(InputTypeColumnName),

                            // join column
                            InspectionTypeName = reader.GetStringValue(InspectionTypeNameColumnName),

                            Active = reader.GetBooleanValue(BaseInfrastructure.ActiveColumnName)
                        };

                        items.Add(item);
                    }

                    if (!reader.IsClosed) reader.Close();
                }
            }

            return items;
        }

        public async Task<bool> Update(InspectionCheckpoint checkpoint)
        {
            var parameters = new List<DbParameter>
            {
                base.GetParameter(InspectionCheckpointIdParameterName,   checkpoint.InspectionCheckpointId),
                base.GetParameter(InspectionTypeIdParameterName,         checkpoint.InspectionTypeId),
                base.GetParameter(InspectionCheckpointNameParameterName, (object?)checkpoint.InspectionCheckpointName ?? DBNull.Value),
                base.GetParameter(InputTypeParameterName,                (object?)checkpoint.InputType ?? DBNull.Value),
                base.GetParameter(ModifiedByIdParameterName,             checkpoint.ModifiedById)
            };

            var rows = await base.ExecuteNonQuery(parameters, UpdateStoredProcedureName, CommandType.StoredProcedure);
            return rows > 0;
        }

        #endregion
    }
}
