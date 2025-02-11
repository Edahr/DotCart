using Microsoft.EntityFrameworkCore;
using Sheyaaka.DAL.Interfaces;
using Sheyaaka.Data;
using Sheyaaka.Data.Models;

namespace Sheyaaka.DAL.Repositories
{
    public class StoreRepository : Repository<Store>, IStoreRepository
    {
        private readonly AppDbContext _context;

        public StoreRepository(AppDbContext context) : base(context) => _context = context;

        // Get all stores by userID 
        public async Task<IEnumerable<Store>?> GetUserStoresAsync(int userId)
        {
            return await _context.Stores.Where(s => s.UserID == userId).ToListAsync();
        }

        // Check if store exists by name
        public async Task<bool> IsStoreExistsAsync(string storeName)
        {
            return await _context.Stores.AnyAsync(s => s.StoreName.ToLower() == storeName.ToLower());
        }
    }
}
