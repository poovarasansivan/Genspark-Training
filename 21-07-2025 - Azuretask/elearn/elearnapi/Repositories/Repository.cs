using ElearnApi.Models;
using ElearnApi.Context;
using ElearnApi.Interface;
using Microsoft.EntityFrameworkCore;

namespace ElearnApi.Repositories
{
    public abstract class Repository<T> : IRepository<T> where T : class
    {
        protected readonly AppDbContext _context;

        public Repository(AppDbContext context)
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
            _context.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity == null)
            {
                throw new ArgumentException(nameof(entity), "Entity cannot be empty");
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
            _context.Set<T>().Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}