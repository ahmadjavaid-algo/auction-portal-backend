// AuctionPortal.InfrastructureLayer.Infrastructure/ModelInfrastructure.cs
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
    public class ModelInfrastructure : BaseInfrastructure, IModelInfrastructure
    {
        #region Constructor
        /// <summary>
        /// ModelInfrastructure initializes class object.
        /// </summary>
        public ModelInfrastructure(IConfiguration configuration, ILogger<ModelInfrastructure> logger)
            : base(configuration, logger)
        {
        }
        #endregion

        #region Constants
        private const string AddStoredProcedureName = "[dbo].[sp_Model_Add]";
        private const string ActivateStoredProcedureName = "[dbo].[sp_Model_Activate]";
        private const string GetStoredProcedureName = "[dbo].[sp_Model_Get]";
        private const string GetListStoredProcedureName = "[dbo].[sp_Model_GetAll]";
        private const string UpdateStoredProcedureName = "[dbo].[sp_Model_Update]";

        private const string ModelIdColumnName = "ModelId";
        private const string MakeIdColumnName = "MakeId";
        private const string ModelNameColumnName = "ModelName";
        private const string MakeNameColumnName = "MakeName";

        private const string ModelIdParameterName = "@ModelId";
        private const string MakeNameParameterName = "@MakeName";
        private const string MakeIdParameterName = "@MakeId";
        private const string ModelNameParameterName = "@ModelName";
        private const string CreatedByIdParameterName = "@CreatedById";
        private const string ModifiedByIdParameterName = "@ModifiedById";
        #endregion

        #region IModelInfrastructure Implementation

        /// <summary>
        /// Add adds new Model and returns generated ModelId.
        /// </summary>
        public async Task<int> Add(Model model)
        {
            var parameters = new List<DbParameter>
            {
                base.GetParameter(MakeIdParameterName, model.MakeId),
                base.GetParameter(ModelNameParameterName, model.ModelName),
                base.GetParameter(CreatedByIdParameterName, model.CreatedById)
            };

            // sp_Model_Add INSERTs and SELECTs the inserted row.
            using (var reader = await base.ExecuteReader(parameters, AddStoredProcedureName, CommandType.StoredProcedure))
            {
                if (reader != null && reader.HasRows && reader.Read())
                {
                    model.ModelId = reader.GetIntegerValue(ModelIdColumnName);
                    model.MakeId = reader.GetIntegerValue(MakeIdColumnName);
                    model.ModelName = reader.GetStringValue(ModelNameColumnName);

                    // Audit (optional to read on Add)
                    model.CreatedById = reader.GetIntegerValueNullable(BaseInfrastructure.CreatedByIdColumnName);
                    model.CreatedDate = reader.GetDateTimeValueNullable(BaseInfrastructure.CreatedDateColumnName);
                    model.ModifiedById = reader.GetIntegerValueNullable(BaseInfrastructure.ModifiedByIdColumnName) ?? 0;
                    model.ModifiedDate = reader.GetDateTimeValueNullable(BaseInfrastructure.ModifiedDateColumnName);
                    model.Active = reader.GetBooleanValue(BaseInfrastructure.ActiveColumnName);
                }

                if (reader != null && !reader.IsClosed)
                    reader.Close();
            }

            return model.ModelId;
        }

        /// <summary>
        /// Activate activate/deactivate provided record and returns true if successful.
        /// </summary>
        public async Task<bool> Activate(Model model)
        {
            var parameters = new List<DbParameter>
            {
                base.GetParameter(ModelIdParameterName, model.ModelId),
                base.GetParameter(BaseInfrastructure.ActiveParameterName, model.Active),
                base.GetParameter(ModifiedByIdParameterName, model.ModifiedById)
            };

            var rows = await base.ExecuteNonQuery(parameters, ActivateStoredProcedureName, CommandType.StoredProcedure);
            return rows > 0;
        }

        /// <summary>
        /// Get fetches and returns a single Model.
        /// </summary>
        public async Task<Model> Get(Model model)
        {
            Model item = null;

            var parameters = new List<DbParameter>
            {
                base.GetParameter(ModelIdParameterName, model.ModelId)
            };

            using (var reader = await base.ExecuteReader(parameters, GetStoredProcedureName, CommandType.StoredProcedure))
            {
                if (reader != null && reader.HasRows && reader.Read())
                {
                    item = new Model
                    {
                        ModelId = reader.GetIntegerValue(ModelIdColumnName),
                        MakeId = reader.GetIntegerValue(MakeIdColumnName),
                        ModelName = reader.GetStringValue(ModelNameColumnName),

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
        /// GetList fetches and returns a list of Models (trimmed columns).
        /// </summary>
        public async Task<List<Model>> GetList(Model model)
        {
            var items = new List<Model>();
            var parameters = new List<DbParameter>(); // none

            using (var reader = await base.ExecuteReader(parameters, GetListStoredProcedureName, CommandType.StoredProcedure))
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        var item = new Model
                        {
                            ModelId = reader.GetIntegerValue(ModelIdColumnName),
                            MakeId = reader.GetIntegerValue(MakeIdColumnName),
                            MakeName = reader.GetStringValue(MakeNameColumnName),
                            ModelName = reader.GetStringValue(ModelNameColumnName),
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
        /// Update updates an existing Model and returns true if successful.
        /// </summary>
        public async Task<bool> Update(Model model)
        {
            var parameters = new List<DbParameter>
            {
                base.GetParameter(ModelIdParameterName, model.ModelId),
                base.GetParameter(ModelNameParameterName, (object?)model.ModelName ?? DBNull.Value),
                
                base.GetParameter(MakeIdParameterName, model.MakeId > 0 ? (object)model.MakeId : DBNull.Value),
                base.GetParameter(ModifiedByIdParameterName, model.ModifiedById)
            };

            var rows = await base.ExecuteNonQuery(parameters, UpdateStoredProcedureName, CommandType.StoredProcedure);
            return rows > 0;
        }


        #endregion
    }
}
