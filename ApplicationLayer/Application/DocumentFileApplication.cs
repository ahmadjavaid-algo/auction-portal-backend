using AuctionPortal.ApplicationLayer.IApplication;
using AuctionPortal.InfrastructureLayer.Interfaces;
using AuctionPortal.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuctionPortal.ApplicationLayer.Application
{
    public class DocumentFileApplication : BaseApplication, IDocumentFileApplication
    {
        public DocumentFileApplication(IDocumentFileInfrastructure DocumentFileInfrastructure, IConfiguration configuration)
            : base(configuration)
        {
            this.DocumentFileInfrastructure = DocumentFileInfrastructure ?? throw new ArgumentNullException(nameof(DocumentFileInfrastructure));
        }

        public IDocumentFileInfrastructure DocumentFileInfrastructure { get; }

        #region Queries
        public async Task<DocumentFile> Get(DocumentFile entity)
        {
            return await DocumentFileInfrastructure.Get(entity);
        }

        public async Task<List<DocumentFile>> GetList(DocumentFile entity)
        {
            return await DocumentFileInfrastructure.GetList(entity);
        }
        #endregion

        #region Commands
        public async Task<int> Add(DocumentFile entity)
        {
            return await DocumentFileInfrastructure.Add(entity);
        }
        public async Task<int> Upload(DocumentFile entity)
        {
            return await DocumentFileInfrastructure.Upload(entity);
        }

        public async Task<bool> Update(DocumentFile entity)
        {
            return await DocumentFileInfrastructure.Update(entity);
        }

        public async Task<bool> Activate(DocumentFile entity)
        {
            return await DocumentFileInfrastructure.Activate(entity);
        }
        #endregion
    }
}
