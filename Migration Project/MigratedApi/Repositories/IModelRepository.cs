using MigratedApi.Models;
using MigratedApi.Contexts;
using Microsoft.EntityFrameworkCore;

namespace MigratedApi.Repositories
{
    public class ModelRepository : Repository<Model>
    {
        public ModelRepository(ShopDbContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<Model>> GetAllAsync()
        {
            return await _context.Models
                .ToListAsync();
        }
        public override async Task<Model> GetByIdAsync(int id)
        {
            var model = await _context.Models
                .FirstOrDefaultAsync(m => m.ModelId == id);
            if (model == null)
            {
                throw new KeyNotFoundException($"Model with id {id} not found.");
            }
            return model;
        }
    }
}