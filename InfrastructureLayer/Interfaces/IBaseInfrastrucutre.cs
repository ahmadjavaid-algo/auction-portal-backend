using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AuctionPortal.InfrastructureLayer.Interfaces
{
    public interface IBaseInfrastructure<T>
    {
        #region Queries
        Task<List<T>> GetList(T entity);
        Task<T> Get(T entity);
        #endregion

        #region Commands
        Task<int> Add(T entity);
        Task<bool> Update(T entity);
        Task<bool> Activate(T entity);
        #endregion
    }
}
