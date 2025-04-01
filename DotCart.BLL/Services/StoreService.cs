using DotCart.BLL.Interfaces;
using DotCart.DAL.Interfaces;
using DotCart.Data.Models;
using DotCart.Dto.Dtos.Stores.Input;

namespace DotCart.BLL.Services
{
    public class StoreService : IStoreService
    {
        private readonly IStoreRepository _storeRepository;

        public StoreService(IStoreRepository storeRepository)
        {
            _storeRepository = storeRepository;
        }

        //Getting a store by Store and user Id 
        public async Task<Store?> GetStoreAsync(int storeId)
        {
            var store = await _storeRepository.GetByIdAsync(storeId);

            return store;
        }

        //Getting Stores by user Id
        public async Task<IEnumerable<Store>?> GetUserStoresAsync(int userId)
        {
            return await _storeRepository.GetUserStoresAsync(userId) ?? null;
        }
        public async Task<IEnumerable<Store>?> GetAllStoresAsync()
        {
            return await _storeRepository.GetAllAsync() ?? null;
        }

        //Creating a new store 
        public async Task<Store?> CreateStoreAsync(CreateStoreInputDto createStoreInputDto, int userId)
        {
            await ValidateStoreCreationInput(createStoreInputDto);

            //Create a new store object
            Store store = new Store();
            store.UserID = userId;
            store.StoreName = createStoreInputDto.StoreName;
            store.LogoURL = createStoreInputDto.LogoURL;
            store.IsActive = createStoreInputDto.IsActive;

            return await _storeRepository.SaveAsync(store);
        }

        //Updating an existing store
        public async Task<Store?> UpdateStoreAsync(UpdateStoreInputDto updateStoreInputDto, int storeId, int userId)
        {
            var store = await ValidateUpdateStoreRequest(updateStoreInputDto, storeId, userId);
            if (store == null) return null; //item not found, or doesn't belong to this user

            //Update store object
            store.StoreName = updateStoreInputDto.StoreName;
            store.LogoURL = updateStoreInputDto.LogoURL;
            store.IsActive = updateStoreInputDto.IsActive;

            return await _storeRepository.SaveAsync(store);
        }

        //Deleting an existing store
        public async Task<Store?> DeleteStoreAsync(int storeId, int userId)
        {
            var store = await _storeRepository.GetByIdAsync(storeId);
            if (store == null || store?.UserID != userId) return null; //item not found or store doesn't belong to user
            return await _storeRepository.DeleteAsync(store) ?? null;
        }

        #region Private Methods
        private async Task ValidateStoreCreationInput(CreateStoreInputDto createStoreInputDto)
        {
            ArgumentNullException.ThrowIfNull(createStoreInputDto, "Store Object is null");
            ArgumentException.ThrowIfNullOrWhiteSpace(createStoreInputDto.StoreName, "Store name is required");

            if (await _storeRepository.IsStoreExistsAsync(createStoreInputDto.StoreName))
                throw new Exception("Store name already exists.");
        }
        private async Task<Store?> ValidateUpdateStoreRequest(UpdateStoreInputDto updateStoreInputDto, int storeId, int userId)
        {
            ArgumentNullException.ThrowIfNull(updateStoreInputDto, "Store Object is null");
            ArgumentException.ThrowIfNullOrWhiteSpace(updateStoreInputDto.StoreName, "Store name is required");

            var store = await _storeRepository.GetByIdAsync(storeId);
            if (store == null || store?.UserID != userId)
                return null;

            if (store.StoreName != updateStoreInputDto.StoreName && await _storeRepository.IsStoreExistsAsync(updateStoreInputDto.StoreName))
                throw new Exception("Store name already exists.");

            return store;
        }

        #endregion
    }
}
