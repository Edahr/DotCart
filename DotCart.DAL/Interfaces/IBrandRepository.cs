using DotCart.Data.Models;

namespace DotCart.DAL.Interfaces
{
    public interface IBrandRepository : IRepository<Brand>
    {
        Task<bool> IsBrandExistsAsync(string brandName);
    }
}
