using Microsoft.EntityFrameworkCore;
using DotCart.Data.Models;
using DotCart.Data;
using DotCart.DAL.Interfaces;

namespace DotCart.DAL.Repositories
{
    public class StoreBrandRepository : Repository<StoreBrand>, IStoreBrandRepository
    {
        private readonly AppDbContext _context;
        public StoreBrandRepository(AppDbContext context) : base(context) => _context = context;

        public async Task<StoreBrand?> GetByBrandIdAndStoreIdAsync(int brandId, int storeId)
        {
            return await _context.StoreBrands.FirstOrDefaultAsync(b => b.BrandID == brandId && b.StoreID == storeId);
        }

        public async Task<StoreBrand?> SaveStoreBrandAsync(StoreBrand storeBrand)
        {
            _context.StoreBrands.Add(storeBrand);
            await _context.SaveChangesAsync();
            return storeBrand;
        }
    }
}
