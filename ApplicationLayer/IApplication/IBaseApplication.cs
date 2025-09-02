using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AuctionPortal.ApplicationLayer.IApplication
{
    /// <summary>
    /// Fully generic application-layer contract for CRUD-style operations.
    /// Generic over both the entity type <typeparamref name="T"/> and the identifier type <typeparamref name="TId"/>.
    /// </summary>
    /// <typeparam name="T">Entity type.</typeparam>
    /// <typeparam name="TId">Identifier type (e.g., Guid, int, string).</typeparam>
    public interface IBaseApplication<T, TId> where T : class
    {
        #region Queries

        /// <summary>
        /// Returns all items of type <typeparamref name="T"/>.
        /// </summary>
        Task<T> Get(T entity);
        Task<List<T>> GetList(T entity);


        #endregion

        #region Commands

        /// <summary>
        /// Creates a new <typeparamref name="T"/> and returns its identifier.
        /// </summary>
        Task<int> Add(T entity);

        /// <summary>
        /// Updates an existing <typeparamref name="T"/>.
        /// </summary>
        Task<bool> Update(T entity);

        /// <summary>
        /// Activates/deactivates an entity by id. Optionally records the modifier id.
        /// </summary>
        Task<bool> Activate(T entity);

        #endregion
    }
}
