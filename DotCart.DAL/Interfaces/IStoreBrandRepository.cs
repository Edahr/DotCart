using DotCart.Data.Models;

namespace DotCart.DAL.Interfaces
{
    public interface IStoreBrandRepository : IRepository<StoreBrand>
    {
        Task<StoreBrand?> GetByBrandIdAndStoreIdAsync(int brandId, int storeId);
        Task<StoreBrand?> SaveStoreBrandAsync(StoreBrand storeBrand);
    }
}
