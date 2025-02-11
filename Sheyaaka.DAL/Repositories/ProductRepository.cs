using Microsoft.EntityFrameworkCore;
using Sheyaaka.DAL.Interfaces;
using Sheyaaka.Data;
using Sheyaaka.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheyaaka.DAL.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly AppDbContext _context;
        public ProductRepository(AppDbContext context) : base(context) => _context = context;

        //This method allows filtration for products.
        public async Task<IEnumerable<Product>> GetProductsByStoreIdAndStatusAndUserIdAsync(int? storeId = null, bool? isDeleted = null, int? userId = null)
        {
            IQueryable<Product> query = _context.Products;

            // Apply store filter if storeId is provided
            if (storeId.HasValue && storeId > 0)
            {
                query = query.Where(p => p.StoreID == storeId);
            }

            // Apply userId filter if userId is provided
            if (userId.HasValue && userId > 0)
            {
                query = query.Where(p => p.Store.UserID == userId);
            }

            // Apply isDeleted filter if provided
            if (isDeleted.HasValue)
            {
                query = query.Where(p => p.IsDeleted == isDeleted.Value);
            }
            else
            {
                // Default behavior: Get only active products
                query = query.Where(p => !p.IsDeleted);
            }

            return await query.ToListAsync();
        }

        public async Task<bool> IsValidProductInformationAsync(int storeId, int brandId, int userId)
        {
            return await _context.StoreBrands.AnyAsync(
                   sb => sb.StoreID == storeId &&
                   sb.BrandID == brandId &&
                   sb.Store.UserID == userId
                );
        }

        public async Task<Product?> ChangeProductDeletionStatusAsync(int productId, bool isDeleted, int userId )
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductID == productId && p.Store.UserID == userId);

            if (product == null) return null;

            product.IsDeleted = isDeleted;

            await _context.SaveChangesAsync();

            return product;
        }
    }
}
