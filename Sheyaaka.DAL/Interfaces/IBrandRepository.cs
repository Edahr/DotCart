using Sheyaaka.Data.Models;

namespace Sheyaaka.DAL.Interfaces
{
    public interface IBrandRepository : IRepository<Brand>
    {
        Task<bool> IsBrandExistsAsync(string brandName);
    }
}
