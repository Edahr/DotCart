namespace Sheyaaka.DAL.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> SaveAsync(T entity);
        Task<T?> DeleteAsync(T entity);
    }
}
