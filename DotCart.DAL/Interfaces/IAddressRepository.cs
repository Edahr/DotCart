using DotCart.Data.Models;

namespace DotCart.DAL.Interfaces
{
    public interface IAddressRepository : IRepository<Address>
    {
        Task<IEnumerable<Address>> GetStoreAddressesAsync(int storeId, int userId);
        Task<Address?> DeleteAddressAsync(int addressId, int userId);
    }
}
