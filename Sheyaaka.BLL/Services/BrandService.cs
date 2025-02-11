using Microsoft.EntityFrameworkCore;
using Sheyaaka.BLL.Interfaces;
using Sheyaaka.DAL.Interfaces;
using Sheyaaka.Data.Models;
using Sheyaaka.Dto.Dtos.Brands.Input;
using Sheyaaka.Dto.Dtos.Stores.Input;

namespace Sheyaaka.BLL.Services
{
    public class BrandService : IBrandService
    {
        private readonly IBrandRepository _brandRepository;
        private readonly IStoreRepository _storeRepository;
        private readonly IStoreBrandRepository _storeBrandRepository;
        public BrandService(IBrandRepository brandRepository, IStoreBrandRepository storeBrandRepository, IStoreRepository storeRepository)
        {
            _brandRepository = brandRepository;
            _storeBrandRepository = storeBrandRepository;
            _storeRepository = storeRepository;
        }

        // Get brand by ID
        public async Task<Brand?> GetBrandByIdAsync(int brandId)
        {
            return await _brandRepository.GetByIdAsync(brandId) ?? null;
        }

        // Get all brands
        public async Task<IEnumerable<Brand>> GetAllBrandsAsync()
        {
            return await _brandRepository.GetAllAsync();
        }

        // Create a new brand 
        public async Task<Brand?> CreateBrandAsync(CreateBrandInputDto createBrandInputDto)
        {
            await ValidateBrandName(createBrandInputDto.BrandName);

            var brand = new Brand();

            brand.BrandName = createBrandInputDto.BrandName;

            return await _brandRepository.SaveAsync(brand) ?? null;
        }

        // Update Brand
        public async Task<Brand?> UpdateBrandAsync(UpdateBrandInputDto updateBrandInputDto, int brandId)
        {
            var brand = await _brandRepository.GetByIdAsync(brandId);
            if (brand == null) return null;

            await ValidateBrandName(updateBrandInputDto.BrandName);

            brand.BrandName = updateBrandInputDto.BrandName;

            return await _brandRepository.SaveAsync(brand) ?? null;
        }

        // Delete Brand
        public async Task<Brand?> DeleteBrandAsync(int brandId)
        {
            var brand = await _brandRepository.GetByIdAsync(brandId);
            if (brand == null) return null;

            return await _brandRepository.DeleteAsync(brand) ?? null;
        }

        // Unassign a brand from a store
        public async Task<StoreBrand?> UnassignBrandFromStore(int brandId, int storeId, int userId)
        {
            //Checking if store belongs to this user
            var store = await _storeRepository.GetByIdAsync(storeId);

            if (store == null || store?.UserID != userId)
                return null;

            //Checking if the store- brand relationship exists
            var storeBrand = await _storeBrandRepository.GetByBrandIdAndStoreIdAsync(brandId, storeId);

            if (storeBrand == null) return null;

            return await _storeBrandRepository.DeleteAsync(storeBrand);
        }

        //Assign a brand to a store
        public async Task<StoreBrand?> AssignBrandToStore(int brandId, int storeId, int userId)
        {
            //Checking if store belongs to this user
            var store = await _storeRepository.GetByIdAsync(storeId);

            if (store == null || store?.UserID != userId)
                return null;

            var brand = await _brandRepository.GetByIdAsync(brandId);
            if (brand == null) return null;

            //Checking if this relationship already exists  => if it does Just return it.
            var storeBrand = await _storeBrandRepository.GetByBrandIdAndStoreIdAsync(brandId, storeId);

            if (storeBrand != null) return storeBrand;

            //Creating new store brand instance
            storeBrand = new StoreBrand();
            storeBrand.BrandID = brandId;
            storeBrand.StoreID = storeId;

            return await _storeBrandRepository.SaveStoreBrandAsync(storeBrand);
        }

        private async Task ValidateBrandName(string brandName)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(brandName, "Brand name is required");

            if (await _brandRepository.IsBrandExistsAsync(brandName))
            {
                throw new InvalidOperationException("Brand name must be unique.");
            }
        }

    }
}
