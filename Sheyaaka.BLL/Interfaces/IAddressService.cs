using Sheyaaka.Data.Models;
using Sheyaaka.Dto.Dtos.Addresses.Input;

namespace Sheyaaka.BLL.Interfaces
{
    public interface IAddressService
    {
        Task<IEnumerable<Address>> GetStoreAddresses(int storeId, int userId);
        Task<Address?> CreateAddressAsync(CreateAddressInputDto createAddressInputDto, int userId);
        Task<Address?> UpdateAddressAsync(UpdateAddressInputDto updateAddressAsync, int addressId, int userId);
        Task<Address?> DeleteAddressAsync(int addressId, int userId);
    }
}
