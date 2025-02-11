using Microsoft.EntityFrameworkCore;
using Sheyaaka.DAL.Interfaces;
using Sheyaaka.Data;
using Sheyaaka.Data.Models;

namespace Sheyaaka.DAL.Repositories
{
    public class AddressRepository : Repository<Address>, IAddressRepository
    {
        private readonly AppDbContext _context;

        public AddressRepository(AppDbContext context) : base(context) => _context = context;


        //Get all addresses by store Id
        public async Task<IEnumerable<Address>> GetStoreAddressesAsync(int storeId, int userId)
        {
            return await _context.Addresses
               .Where(a => a.StoreID == storeId && a.Store.UserID == userId)
               .ToListAsync();
        }

        //This will ensure deletion takes only 1 single sql query
        public async Task<Address?> DeleteAddressAsync(int addressId, int userId)
        {
            var address = await _context.Addresses
                .Where(a => a.AddressID == addressId && a.Store.UserID == userId)
                .FirstOrDefaultAsync();

            if (address == null) return null; // Address not found or doesn't belong to store

            _context.Addresses.Remove(address);
            await _context.SaveChangesAsync();
            return address;
        }
    }
}
