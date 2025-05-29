using Microsoft.EntityFrameworkCore;
using BankingAPI.Interfaces;
using BankingAPI.Contexts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankingAPI.Repositories
{
    public abstract class Repository<K, T> : IRepository<K, T> where T : class
    {
        protected readonly BankingContext _context;

        public Repository(BankingContext context)
        {
            _context = context;
        }

        public abstract Task<T> GetByIdAsync(K id);

        public abstract Task<ICollection<T>> GetAllAsync();

        public virtual async Task<T> AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public virtual async Task<T> UpdateAsync(T entity)
        {
            _context.Set<T>().Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public virtual async Task<T> DeleteAsync(K id)
        {
            var entity = await GetByIdAsync(id);
            if (entity == null)
                return null;

            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
    }
}
