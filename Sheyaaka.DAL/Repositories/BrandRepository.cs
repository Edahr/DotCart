using Microsoft.EntityFrameworkCore;
using Sheyaaka.DAL.Interfaces;
using Sheyaaka.Data;
using Sheyaaka.Data.Models;

namespace Sheyaaka.DAL.Repositories
{
    public class BrandRepository : Repository<Brand>, IBrandRepository
    {
        private readonly AppDbContext _context;
        public BrandRepository(AppDbContext context) : base(context) => _context = context;

        public async Task<bool> IsBrandExistsAsync(string brandName)
        {
            return await _context.Brands.AnyAsync(b => b.BrandName.ToLower() == brandName.ToLower());
        }
    }
}
