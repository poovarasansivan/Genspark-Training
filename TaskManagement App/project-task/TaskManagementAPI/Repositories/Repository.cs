using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Data;
using TaskManagementAPI.Interfaces;
using TaskManagementAPI.Models.Base;
using TaskManagementAPI.CustomExceptions; 

namespace TaskManagementAPI.Repositories
{
    public abstract class Repository<K, T> : IRepository<K, T> where T : AuditBase
    {
        protected readonly TaskManagementDbContext _context;

        public Repository(TaskManagementDbContext context)
        {
            _context = context;
        }

        public async Task<T> Add(T item)
        {
            _context.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public abstract Task<T?> Get(K key);

        public abstract Task<IEnumerable<T>> GetAll();

        public async Task<T> Update(K key, T item)
        {
            var existing = await Get(key);
            if (existing == null)
                throw new NotFoundException($"No item found with ID = {key} for update");

            _context.Entry(existing).CurrentValues.SetValues(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<T> Delete(K key)
        {
            var item = await Get(key);
            if (item == null)
                throw new NotFoundException($"No item found with ID = {key} for deletion");

            item.IsDeleted = true;
            item.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return item;
        }
    }
}
