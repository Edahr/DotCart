using Sheyaaka.Data.Models;

namespace Sheyaaka.DAL.Interfaces
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<IEnumerable<Product>> GetProductsByStoreIdAndStatusAndUserIdAsync(int? storeId = null, bool? isDeleted = null, int? userId = null);
        Task<bool> IsValidProductInformationAsync(int storeId, int brandId, int userId);
        Task<Product?> ChangeProductDeletionStatusAsync(int productId, bool isDeleted, int userId);
    }
}
