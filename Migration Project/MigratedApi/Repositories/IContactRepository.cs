using MigratedApi.Models;
using MigratedApi.Contexts;
using Microsoft.EntityFrameworkCore;

namespace MigratedApi.Repositories
{
    public class ContactRepository : Repository<Contact>
    {
        public ContactRepository(ShopDbContext context) : base(context)
        {
        }
        public override Task<IEnumerable<Contact>> GetAllAsync()
        {
            return Task.FromResult(_context.Set<Contact>().AsEnumerable());
        }

        public override Task<Contact> GetByIdAsync(int id)
        {
            var entity = _context.Set<Contact>().Find(id);
            if (entity == null)
            {
                throw new KeyNotFoundException($"Contact with id {id} not found.");
            }
            return Task.FromResult(entity);
        }
    }
}