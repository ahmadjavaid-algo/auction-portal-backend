// AuctionPortal.InfrastructureLayer.Interfaces/IDocumentFileInfrastructure.cs
using AuctionPortal.InfrastructureLayer.Infrastructure;
using AuctionPortal.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuctionPortal.InfrastructureLayer.Interfaces
{
    public interface IDocumentFileInfrastructure:IBaseInfrastructure<DocumentFile>
    {
        Task<int> Upload(DocumentFile entity);
    }
}
