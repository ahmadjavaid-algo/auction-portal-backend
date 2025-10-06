using AuctionPortal.Models;

namespace AuctionPortal.ApplicationLayer.IApplication
{
    public interface IDocumentFileApplication : IBaseApplication<DocumentFile, int>
    {
        Task<int> Upload(DocumentFile entity);
    }
}
