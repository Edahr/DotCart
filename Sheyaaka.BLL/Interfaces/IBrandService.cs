using Sheyaaka.Data.Models;
using Sheyaaka.Dto.Dtos.Brands.Input;

namespace Sheyaaka.BLL.Interfaces
{
    public interface IBrandService
    {
        Task<Brand?> GetBrandByIdAsync(int brandId);
        Task<IEnumerable<Brand>> GetAllBrandsAsync();
        Task<Brand?> CreateBrandAsync(CreateBrandInputDto createBrandInputDto);
        Task<Brand?> UpdateBrandAsync(UpdateBrandInputDto updateBrandInputDto, int brandId);
        Task<Brand?> DeleteBrandAsync(int brandId);
        Task<StoreBrand?> UnassignBrandFromStore(int brandId, int storeId, int userId);
        Task<StoreBrand?> AssignBrandToStore(int brandId, int storeId, int userId);
    }
}
