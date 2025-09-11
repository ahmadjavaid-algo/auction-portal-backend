using AuctionPortal.Models;

namespace AuctionPortal.InfrastructureLayer.Interfaces
{
    public interface IEmailsInfrastructure : IBaseInfrastructure<Email>
    {
        Task<Email?> GetByCode(string code);
    }
}
