using Sheyaaka.Data.Models;

namespace Sheyaaka.DAL.Interfaces
{
    public interface IStoreBrandRepository : IRepository<StoreBrand>
    {
        Task<StoreBrand?> GetByBrandIdAndStoreIdAsync(int brandId, int storeId);
        Task<StoreBrand?> SaveStoreBrandAsync(StoreBrand storeBrand);
    }
}
