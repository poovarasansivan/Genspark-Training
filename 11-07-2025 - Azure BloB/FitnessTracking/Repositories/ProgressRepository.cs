using FitnessTracking.Models.DTOs;
using FitnessTracking.Models;
using FitnessTracking.Interfaces;
using FitnessTracking.Contexts;
using Microsoft.EntityFrameworkCore;

namespace FitnessTracking.Repositories
{
    public class ProgressRepository : Repository<ProgressModel>
    {
        public ProgressRepository(FitnessContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<ProgressModel>> GetAllAsync()
        {
            return await _context.Set<ProgressModel>().ToListAsync();
        }

        public override async Task<ProgressModel?> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Progress ID cannot be empty or null", nameof(id));
            return await _context.Set<ProgressModel>().FindAsync(id);
        }
    }
}