// AuctionPortal.InfrastructureLayer.Infrastructure/InspectionTypeInfrastructure.cs
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
    public class InspectionTypeInfrastructure : BaseInfrastructure, IInspectionTypeInfrastructure
    {
        #region Constructor
        public InspectionTypeInfrastructure(IConfiguration configuration, ILogger<InspectionTypeInfrastructure> logger)
            : base(configuration, logger) { }
        #endregion

        #region Constants
        private const string AddStoredProcedureName = "[dbo].[sp_InspectionType_Add]";
        private const string ActivateStoredProcedureName = "[dbo].[sp_InspectionType_Activate]";
        private const string GetStoredProcedureName = "[dbo].[sp_InspectionType_Get]";
        private const string GetListStoredProcedureName = "[dbo].[sp_InspectionType_GetAll]";
        private const string UpdateStoredProcedureName = "[dbo].[sp_InspectionType_Update]";

        private const string InspectionTypeIdColumnName = "InspectionTypeId";
        private const string InspectionTypeNameColumnName = "InspectionTypeName";
        private const string WeightageColumnName = "Weightage";

        private const string InspectionTypeIdParameterName = "@InspectionTypeId";
        private const string InspectionTypeNameParameterName = "@InspectionTypeName";
        private const string WeightageParameterName = "@Weightage";
        private const string CreatedByIdParameterName = "@CreatedById";
        private const string ModifiedByIdParameterName = "@ModifiedById";
        #endregion

        #region IInspectionTypeInfrastructure Implementation

        public async Task<int> Add(InspectionType inspectionType)
        {
            var parameters = new List<DbParameter>
            {
                base.GetParameter(InspectionTypeNameParameterName, inspectionType.InspectionTypeName),
                base.GetParameter(WeightageParameterName,          inspectionType.Weightage),
                base.GetParameter(CreatedByIdParameterName,        inspectionType.CreatedById)
            };

            using (var reader = await base.ExecuteReader(parameters, AddStoredProcedureName, CommandType.StoredProcedure))
            {
                if (reader != null && reader.HasRows && reader.Read())
                {
                    inspectionType.InspectionTypeId = reader.GetIntegerValue(InspectionTypeIdColumnName);
                    inspectionType.InspectionTypeName = reader.GetStringValue(InspectionTypeNameColumnName);
                    inspectionType.Weightage = reader.GetIntegerValue(WeightageColumnName);

                    // Audit
                    inspectionType.CreatedById = reader.GetIntegerValueNullable(BaseInfrastructure.CreatedByIdColumnName);
                    inspectionType.CreatedDate = reader.GetDateTimeValueNullable(BaseInfrastructure.CreatedDateColumnName);
                    inspectionType.ModifiedById = reader.GetIntegerValueNullable(BaseInfrastructure.ModifiedByIdColumnName) ?? 0;
                    inspectionType.ModifiedDate = reader.GetDateTimeValueNullable(BaseInfrastructure.ModifiedDateColumnName);
                    inspectionType.Active = reader.GetBooleanValue(BaseInfrastructure.ActiveColumnName);
                }

                if (reader != null && !reader.IsClosed) reader.Close();
            }

            return inspectionType.InspectionTypeId;
        }

        public async Task<bool> Activate(InspectionType inspectionType)
        {
            var parameters = new List<DbParameter>
            {
                base.GetParameter(InspectionTypeIdParameterName,      inspectionType.InspectionTypeId),
                base.GetParameter(BaseInfrastructure.ActiveParameterName, inspectionType.Active),
                base.GetParameter(ModifiedByIdParameterName,          inspectionType.ModifiedById)
            };

            var rows = await base.ExecuteNonQuery(parameters, ActivateStoredProcedureName, CommandType.StoredProcedure);
            return rows > 0;
        }

        public async Task<InspectionType> Get(InspectionType inspectionType)
        {
            InspectionType item = null;

            var parameters = new List<DbParameter>
            {
                base.GetParameter(InspectionTypeIdParameterName, inspectionType.InspectionTypeId)
            };

            using (var reader = await base.ExecuteReader(parameters, GetStoredProcedureName, CommandType.StoredProcedure))
            {
                if (reader != null && reader.HasRows && reader.Read())
                {
                    item = new InspectionType
                    {
                        InspectionTypeId = reader.GetIntegerValue(InspectionTypeIdColumnName),
                        InspectionTypeName = reader.GetStringValue(InspectionTypeNameColumnName),
                        Weightage = reader.GetIntegerValue(WeightageColumnName),

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

        public async Task<List<InspectionType>> GetList(InspectionType inspectionType)
        {
            var items = new List<InspectionType>();
            var parameters = new List<DbParameter>(); // none

            using (var reader = await base.ExecuteReader(parameters, GetListStoredProcedureName, CommandType.StoredProcedure))
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        var item = new InspectionType
                        {
                            InspectionTypeId = reader.GetIntegerValue(InspectionTypeIdColumnName),
                            InspectionTypeName = reader.GetStringValue(InspectionTypeNameColumnName),
                            Weightage = reader.GetIntegerValue(WeightageColumnName),
                            Active = reader.GetBooleanValue(BaseInfrastructure.ActiveColumnName)
                        };

                        items.Add(item);
                    }

                    if (!reader.IsClosed) reader.Close();
                }
            }

            return items;
        }

        public async Task<bool> Update(InspectionType inspectionType)
        {
            var parameters = new List<DbParameter>
            {
                base.GetParameter(InspectionTypeIdParameterName,      inspectionType.InspectionTypeId),
                base.GetParameter(InspectionTypeNameParameterName,    (object?)inspectionType.InspectionTypeName ?? DBNull.Value),
                base.GetParameter(WeightageParameterName,             inspectionType.Weightage),
                base.GetParameter(ModifiedByIdParameterName,          inspectionType.ModifiedById)
            };

            var rows = await base.ExecuteNonQuery(parameters, UpdateStoredProcedureName, CommandType.StoredProcedure);
            return rows > 0;
        }

        #endregion
    }
}
