using DotCart.Data.Models;
using DotCart.Dto.Dtos.Brands.Input;

namespace DotCart.BLL.Interfaces
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
