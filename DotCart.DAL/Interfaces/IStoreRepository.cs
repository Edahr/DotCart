using DotCart.Data.Models;

namespace DotCart.DAL.Interfaces
{
    public interface IStoreRepository : IRepository<Store>
    {
        Task<IEnumerable<Store>?> GetUserStoresAsync(int userId);
        Task<bool> IsStoreExistsAsync(string storeName);
    }
}
