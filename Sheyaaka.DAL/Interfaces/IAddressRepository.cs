using Sheyaaka.Data.Models;

namespace Sheyaaka.DAL.Interfaces
{
    public interface IAddressRepository : IRepository<Address>
    {
        Task<IEnumerable<Address>> GetStoreAddressesAsync(int storeId, int userId);
        Task<Address?> DeleteAddressAsync(int addressId, int userId);
    }
}
