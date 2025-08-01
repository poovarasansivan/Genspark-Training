using MigratedApi.Models;
using MigratedApi.Models.Dtos;
using MigratedApi.Interfaces;
using MigratedApi.Contexts;
using Microsoft.EntityFrameworkCore;

namespace MigratedApi.Repositories
{
    public abstract class Repository<T> : IRepository<T> where T : class
    {
        protected readonly ShopDbContext _context;

        public Repository(ShopDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentException(nameof(entity), "Entity cannot be empty");
            }
            if (_context.Set<T>().Local.Any(e => e == entity))
            {
                return;
            }
            _context.Set<T>().Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = _context.Set<T>().Find(id);
            if (entity == null)
            {
                throw new KeyNotFoundException($"Entity with id {id} not found.");
            }
            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();
        }

        public abstract Task<IEnumerable<T>> GetAllAsync();
        public abstract Task<T> GetByIdAsync(int id);

        public async Task UpdateAsync(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentException(nameof(entity), "Entity cannot be empty");
            }
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}