using MigratedApi.Models;
using MigratedApi.Contexts;
using Microsoft.EntityFrameworkCore;

namespace MigratedApi.Repositories
{
    public class ColorRepository : Repository<Color> 
    {
        public ColorRepository(ShopDbContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<Color>> GetAllAsync()
        {
            return await _context.Set<Color>().ToListAsync();
        }

        public override async Task<Color> GetByIdAsync(int id)
        {
            var entity = await _context.Set<Color>().FindAsync(id);
            if (entity == null)
            {
                throw new KeyNotFoundException($"Color with id {id} not found.");
            }
            return entity;
        }
    }
}