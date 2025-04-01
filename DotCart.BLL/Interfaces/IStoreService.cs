using DotCart.Data.Models;
using DotCart.Dto.Dtos.Stores.Input;

namespace DotCart.BLL.Interfaces
{
    public interface IStoreService
    {
        Task<Store?> GetStoreAsync(int storeId);
        Task<IEnumerable<Store>?> GetUserStoresAsync(int userId);
        Task<IEnumerable<Store>?> GetAllStoresAsync();
        Task<Store?> CreateStoreAsync(CreateStoreInputDto createStoreInputDto, int userId);
        Task<Store?> UpdateStoreAsync(UpdateStoreInputDto updateStoreInputDto, int storeId, int userId);
        Task<Store?> DeleteStoreAsync(int storeId, int userId);
    }
}
