using BankingApp.API.BankingApp.Infrastructure.Data;
using BankingApp.API.BankingApp.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BankingApp.API.BankingApp.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly BankingDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(BankingDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();

        public async Task<T?> GetByIdAsync(Guid id) => await _dbSet.FindAsync(id);

        public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);

        public void Update(T entity) => _dbSet.Update(entity);

        public void UpdateRange(IEnumerable<T> entities)
        {
            _context.Set<T>().UpdateRange(entities);
        }

        public void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }
    }

}
