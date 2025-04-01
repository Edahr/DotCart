using DotCart.DAL.Interfaces;
using DotCart.Data;
using DotCart.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace DotCart.DAL.Repositories
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
