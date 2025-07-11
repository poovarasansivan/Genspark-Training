using FitnessTracking.Models;
using FitnessTracking.Contexts;
using Microsoft.EntityFrameworkCore;

namespace FitnessTracking.Repositories
{
    public class ProgressImageRepository : Repository<ProgressImageModel>
    {
        public ProgressImageRepository(FitnessContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<ProgressImageModel>> GetAllAsync()
        {
            return await _context.Set<ProgressImageModel>().ToListAsync();
        }

        public override async Task<ProgressImageModel?> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Progress Image ID cannot be empty or null", nameof(id));
            return await _context.Set<ProgressImageModel>().FindAsync(id);
        }
    }
}