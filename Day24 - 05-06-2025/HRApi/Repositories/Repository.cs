using HRApi.Models;
using HRApi.Interfaces;
using Microsoft.EntityFrameworkCore;
using HRApi.Contexts;

namespace HRApi.Repositories
{
    public abstract class Repository<K, T> : IRepository<K, T> where T : class
    {
        protected readonly DBContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(DBContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<T> Add(T item)
        {
            await _dbSet.AddAsync(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public abstract Task<T> Get(K key);

        public abstract Task<IEnumerable<T>> GetAll();

        public async Task<T> Update(K key, T item)
        {
            var existingItem = await Get(key);
            if (existingItem == null) return null;

            _context.Entry(existingItem).CurrentValues.SetValues(item);
            await _context.SaveChangesAsync();
            return existingItem;
        }

        public async Task<T> Delete(K key)
        {
            var item = await Get(key);
            if (item == null) return null;

            _dbSet.Remove(item);
            await _context.SaveChangesAsync();
            return item;
        }
    }
}