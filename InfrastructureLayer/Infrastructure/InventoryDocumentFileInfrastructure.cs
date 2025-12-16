// AuctionPortal.InfrastructureLayer.Infrastructure/InventoryDocumentFileInfrastructure.cs
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
    public class InventoryDocumentFileInfrastructure : BaseInfrastructure, IInventoryDocumentFileInfrastructure
    {
        #region Constructor
        public InventoryDocumentFileInfrastructure(IConfiguration configuration, ILogger<InventoryDocumentFileInfrastructure> logger)
            : base(configuration, logger) { }
        #endregion

        #region Constants
        private const string AddStoredProcedureName = "[dbo].[sp_InventoryDocumentFile_Add]";
        private const string ActivateStoredProcedureName = "[dbo].[sp_InventoryDocumentFile_Activate]";
        private const string GetStoredProcedureName = "[dbo].[sp_InventoryDocumentFile_Get]";
        private const string GetListStoredProcedureName = "[dbo].[sp_InventoryDocumentFile_GetAll]";
        private const string UpdateStoredProcedureName = "[dbo].[sp_InventoryDocumentFile_Update]";

        // Columns (base table)
        private const string InventoryDocumentFileIdColumnName = "InventoryDocumentFileId";
        private const string DocumentFileIdColumnName = "DocumentFileId";
        private const string InventoryIdColumnName = "InventoryId";
        private const string DocumentDisplayNameColumnName = "DocumentDisplayName";

        // Optional projected columns (from joins in GetAll; safe reads)
        private const string DocumentNameColumnName = "DocumentName";
        private const string InventoryDisplayNameColumnName = "DisplayName";
        private const string ChassisNoColumnName = "ChassisNo";
        private const string RegistrationNoColumnName = "RegistrationNo";
        private const string DocumentUrlColumnName = "DocumentUrl";
        private const string DocumentThumbnailUrlColumnName = "DocumentThumbnailUrl";
        private const string DocumentExtensionColumnName = "DocumentExtension";

        // Parameters
        private const string InventoryDocumentFileIdParameterName = "@InventoryDocumentFileId";
        private const string DocumentFileIdParameterName = "@DocumentFileId";
        private const string InventoryIdParameterName = "@InventoryId";
        private const string DocumentDisplayNameParameterName = "@DocumentDisplayName";
        private const string CreatedByIdParameterName = "@CreatedById";
        private const string ModifiedByIdParameterName = "@ModifiedById";
        #endregion

        #region IInventoryDocumentFileInfrastructure Implementation

        /// <summary>
        /// Add new InventoryDocumentFile and return generated key.
        /// SP returns the inserted row (base columns + audit).
        /// </summary>
        public async Task<int> Add(InventoryDocumentFile entity)
        {
            var parameters = new List<DbParameter>
            {
                base.GetParameter(DocumentFileIdParameterName,      entity.DocumentFileId),
                base.GetParameter(InventoryIdParameterName,         entity.InventoryId),
                base.GetParameter(DocumentDisplayNameParameterName, (object?)entity.DocumentDisplayName ?? DBNull.Value),
                base.GetParameter(CreatedByIdParameterName,         entity.CreatedById)
            };

            using (var reader = await base.ExecuteReader(parameters, AddStoredProcedureName, CommandType.StoredProcedure))
            {
                if (reader != null && reader.HasRows && reader.Read())
                {
                    entity.InventoryDocumentFileId = reader.GetIntegerValue(InventoryDocumentFileIdColumnName);
                    entity.DocumentFileId = reader.GetIntegerValue(DocumentFileIdColumnName);
                    entity.InventoryId = reader.GetIntegerValue(InventoryIdColumnName);
                    entity.DocumentDisplayName = reader.GetStringValue(DocumentDisplayNameColumnName);

                    // Audit
                    entity.CreatedById = reader.GetIntegerValueNullable(BaseInfrastructure.CreatedByIdColumnName);
                    entity.CreatedDate = reader.GetDateTimeValueNullable(BaseInfrastructure.CreatedDateColumnName);
                    entity.ModifiedById = reader.GetIntegerValueNullable(BaseInfrastructure.ModifiedByIdColumnName) ?? 0;
                    entity.ModifiedDate = reader.GetDateTimeValueNullable(BaseInfrastructure.ModifiedDateColumnName);
                    entity.Active = reader.GetBooleanValue(BaseInfrastructure.ActiveColumnName);
                }

                if (reader != null && !reader.IsClosed) reader.Close();
            }

            return entity.InventoryDocumentFileId;
        }

        /// <summary>
        /// Activate/deactivate provided record.
        /// </summary>
        public async Task<bool> Activate(InventoryDocumentFile entity)
        {
            var parameters = new List<DbParameter>
            {
                base.GetParameter(InventoryDocumentFileIdParameterName, entity.InventoryDocumentFileId),
                base.GetParameter(BaseInfrastructure.ActiveParameterName, entity.Active),
                base.GetParameter(ModifiedByIdParameterName,              entity.ModifiedById)
            };

            var rows = await base.ExecuteNonQuery(parameters, ActivateStoredProcedureName, CommandType.StoredProcedure);
            return rows > 0;
        }

        /// <summary>
        /// Get one InventoryDocumentFile by id.
        /// </summary>
        public async Task<InventoryDocumentFile> Get(InventoryDocumentFile entity)
        {
            InventoryDocumentFile item = null;

            var parameters = new List<DbParameter>
            {
                base.GetParameter(InventoryDocumentFileIdParameterName, entity.InventoryDocumentFileId)
            };

            using (var reader = await base.ExecuteReader(parameters, GetStoredProcedureName, CommandType.StoredProcedure))
            {
                if (reader != null && reader.HasRows && reader.Read())
                {
                    item = new InventoryDocumentFile
                    {
                        InventoryDocumentFileId = reader.GetIntegerValue(InventoryDocumentFileIdColumnName),
                        DocumentFileId = reader.GetIntegerValue(DocumentFileIdColumnName),
                        InventoryId = reader.GetIntegerValue(InventoryIdColumnName),
                        DocumentDisplayName = reader.GetStringValue(DocumentDisplayNameColumnName),

                        // Audit
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

        /// <summary>
        /// Get list of InventoryDocumentFile (trimmed cols).
        /// SP joins DocumentFile to include DocumentName + URLs.
        /// </summary>
        public async Task<List<InventoryDocumentFile>> GetList(InventoryDocumentFile entity)
        {
            var items = new List<InventoryDocumentFile>();
            var parameters = new List<DbParameter>(); // none

            using (var reader = await base.ExecuteReader(parameters, GetListStoredProcedureName, CommandType.StoredProcedure))
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        var item = new InventoryDocumentFile
                        {
                            InventoryDocumentFileId = reader.GetIntegerValue(InventoryDocumentFileIdColumnName),
                            DocumentFileId = reader.GetIntegerValue(DocumentFileIdColumnName),
                            InventoryId = reader.GetIntegerValue(InventoryIdColumnName),
                            DocumentDisplayName = reader.GetStringValue(DocumentDisplayNameColumnName),

                            // Optional joined projections (safe reads)
                            DocumentName = reader.GetStringValue(DocumentNameColumnName),
                            DocumentUrl = reader.GetStringValue(DocumentUrlColumnName),
                            DocumentThumbnailUrl = reader.GetStringValue(DocumentThumbnailUrlColumnName),
                            DocumentExtension = reader.GetStringValue(DocumentExtensionColumnName),

                            Active = reader.GetBooleanValue(BaseInfrastructure.ActiveColumnName)
                        };

                        items.Add(item);
                    }

                    if (!reader.IsClosed) reader.Close();
                }
            }

            return items;
        }

        /// <summary>
        /// Update an existing InventoryDocumentFile
        /// </summary>
        public async Task<bool> Update(InventoryDocumentFile entity)
        {
            var parameters = new List<DbParameter>
            {
                base.GetParameter(InventoryDocumentFileIdParameterName, entity.InventoryDocumentFileId),

                base.GetParameter(DocumentFileIdParameterName,      entity.DocumentFileId  > 0 ? (object)entity.DocumentFileId  : DBNull.Value),
                base.GetParameter(InventoryIdParameterName,         entity.InventoryId     > 0 ? (object)entity.InventoryId     : DBNull.Value),
                base.GetParameter(DocumentDisplayNameParameterName, (object?)entity.DocumentDisplayName ?? DBNull.Value),

                base.GetParameter(ModifiedByIdParameterName,        entity.ModifiedById)
            };

            var rows = await base.ExecuteNonQuery(parameters, UpdateStoredProcedureName, CommandType.StoredProcedure);
            return rows > 0;
        }

        #endregion
    }
}
