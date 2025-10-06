// AuctionPortal.InfrastructureLayer.Infrastructure/DocumentFileInfrastructure.cs
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
    public class DocumentFileInfrastructure : BaseInfrastructure, IDocumentFileInfrastructure
    {

        public DocumentFileInfrastructure(IConfiguration configuration, ILogger<DocumentFileInfrastructure> logger)
            : base(configuration, logger)
        {
        }


        #region Stored procedure + column/parameter names

        // REAL one you’ll wire first
        private const string UploadStoredProcedureName = "[dbo].[sp_DocumentFile_Upload]";

        // If/when you implement the rest later:
        private const string AddStoredProcedureName = "[dbo].[sp_DocumentFile_Add]";
        private const string UpdateStoredProcedureName = "[dbo].[sp_DocumentFile_Update]";
        private const string ActivateStoredProcedureName = "[dbo].[sp_DocumentFile_Activate]";
        private const string GetStoredProcedureName = "[dbo].[sp_DocumentFile_Get]";
        private const string GetListStoredProcedureName = "[dbo].[sp_DocumentFile_GetAll]";

        // Columns returned by the SPs
        private const string DocumentFileIdColumnName = "DocumentFileId";
        private const string DocumentNameColumnName = "DocumentName";
        private const string DocumentTypeIdColumnName = "DocumentTypeId";
        private const string DocumentExtensionColumnName = "DocumentExtension";
        private const string DocumentUrlColumnName = "DocumentUrl";

        // Optional joined/projection
        private const string DocumentTypeNameColumnName = "DocumentTypeName";

        // Parameters (match the SP signatures you’ll create)
        private const string DocumentFileIdParameterName = "@DocumentFileId";
        private const string DocumentNameParameterName = "@DocumentName";
        private const string DocumentTypeIdParameterName = "@DocumentTypeId";
        private const string DocumentExtensionParameterName = "@DocumentExtension";
        private const string DocumentUrlParameterName = "@DocumentUrl";
        private const string CreatedByIdParameterName = "@CreatedById";
        private const string ModifiedByIdParameterName = "@ModifiedById";
        #endregion

        #region Upload (REAL)

        /// <summary>
        /// Uploads a document record (metadata). Returns generated DocumentFileId.
        /// Expected behavior (like your Auction Add):
        /// - SP inserts row and SELECTs the inserted row including audit/active and optional joins.
        /// </summary>
        public async Task<int> Upload(DocumentFile entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            var parameters = new List<DbParameter>
            {
                base.GetParameter(DocumentNameParameterName,      entity.DocumentName),
                base.GetParameter(DocumentTypeIdParameterName,    entity.DocumentTypeId),
                base.GetParameter(DocumentExtensionParameterName, (object?)entity.DocumentExtension ?? DBNull.Value),
                base.GetParameter(DocumentUrlParameterName,       (object?)entity.DocumentUrl      ?? DBNull.Value),
                base.GetParameter(CreatedByIdParameterName,       entity.CreatedById)
            };

            using (var reader = await base.ExecuteReader(parameters, UploadStoredProcedureName, CommandType.StoredProcedure))
            {
                if (reader != null && reader.HasRows && reader.Read())
                {
                    entity.DocumentFileId = reader.GetIntegerValue(DocumentFileIdColumnName);
                    entity.DocumentName = reader.GetStringValue(DocumentNameColumnName);
                    entity.DocumentTypeId = reader.GetIntegerValue(DocumentTypeIdColumnName);
                    entity.DocumentExtension = reader.GetStringValue(DocumentExtensionColumnName);
                    entity.DocumentUrl = reader.GetStringValue(DocumentUrlColumnName);

                    // Optional projection (if your SP SELECTs it)
                    entity.DocumentTypeName = reader.GetStringValue(DocumentTypeNameColumnName);

                    // Audit (BaseModel)
                    entity.CreatedById = reader.GetIntegerValueNullable(BaseInfrastructure.CreatedByIdColumnName);
                    entity.CreatedDate = reader.GetDateTimeValueNullable(BaseInfrastructure.CreatedDateColumnName);
                    entity.ModifiedById = reader.GetIntegerValueNullable(BaseInfrastructure.ModifiedByIdColumnName) ?? 0;
                    entity.ModifiedDate = reader.GetDateTimeValueNullable(BaseInfrastructure.ModifiedDateColumnName);
                    entity.Active = reader.GetBooleanValue(BaseInfrastructure.ActiveColumnName);
                }

                if (reader != null && !reader.IsClosed)
                    reader.Close();
            }

            return entity.DocumentFileId;
        }
        #endregion

        #region DUMMIES (placeholders to satisfy application contracts)

        public Task<int> Add(DocumentFile entity)
        {
            // Intentionally a placeholder for now
            Logger?.LogInformation("DocumentFileInfrastructure.Add called as dummy.");
            return Task.FromResult(0);
        }

        public Task<bool> Update(DocumentFile entity)
        {
            Logger?.LogInformation("DocumentFileInfrastructure.Update called as dummy.");
            return Task.FromResult(false);
        }

        public Task<bool> Activate(DocumentFile entity)
        {
            Logger?.LogInformation("DocumentFileInfrastructure.Activate called as dummy.");
            return Task.FromResult(false);
        }

        public Task<DocumentFile> Get(DocumentFile entity)
        {
            Logger?.LogInformation("DocumentFileInfrastructure.Get called as dummy.");
            return Task.FromResult<DocumentFile>(null);
        }

        public Task<List<DocumentFile>> GetList(DocumentFile entity)
        {
            Logger?.LogInformation("DocumentFileInfrastructure.GetList called as dummy.");
            return Task.FromResult(new List<DocumentFile>());
        }
        #endregion
    }
}
