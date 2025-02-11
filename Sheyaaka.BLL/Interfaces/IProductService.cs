using Sheyaaka.Data.Models;
using Sheyaaka.Dto.Dtos.Products.Input;

namespace Sheyaaka.BLL.Interfaces
{
    public interface IProductService
    {
        Task<Product?> ChangeProductDeletionStatusAsync(int productId, bool isDeleted, int userId);
        Task<Product?> CreateProductAsync(CreateProductInputDto createProductInputDto, int userId);
        Task<IEnumerable<Product>> GetProductsByStoreIdAndStatusAsync(int? storeId = null, bool? isDeleted = false, int? userId = null);
        Task<Product?> UpdateProductAsync(UpdateProductInputDto updateProductInputDto, int productId, int userId);
    }
}
