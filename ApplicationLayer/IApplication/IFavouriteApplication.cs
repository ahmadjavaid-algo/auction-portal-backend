using System.Threading.Tasks;
using AuctionPortal.Models;

namespace AuctionPortal.ApplicationLayer.IApplication
{
    public interface IFavouriteApplication : IBaseApplication<Favourite, int>
    {
        /// <summary>
        /// Adds a favourite for the specified user and handles any domain logic
        /// such as notifications.
        /// </summary>
        Task<int> AddForUser(Favourite entity, int userId);

        /// <summary>
        /// Toggles the Active flag for the specified favourite for a user
        /// and handles any domain logic such as notifications.
        /// </summary>
        Task<bool> ToggleActiveForUser(Favourite entity, int userId);
    }
}
