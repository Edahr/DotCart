using Microsoft.EntityFrameworkCore;
using Sheyaaka.DAL.Interfaces;
using Sheyaaka.Data;

namespace Sheyaaka.DAL.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly AppDbContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = _context.Set<T>();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        //This method is used to save or update an entity
        //it will check if the entity has a primary key set; 
        //if it has a primary key set, it will update the entity, 
        //otherwise, it will add the entity to the database.
        public async Task<T> SaveAsync(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            _context.Entry(entity).State = _context.Entry(entity).IsKeySet ? EntityState.Modified : EntityState.Added;
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<T?> DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
    }
}
