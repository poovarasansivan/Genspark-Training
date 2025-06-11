using FitnessTracking.Models;
using FitnessTracking.Contexts;
using Microsoft.EntityFrameworkCore;

namespace FitnessTracking.Repositories
{
    public class WorkOutLogRepository : Repository<WorkoutModel>
    {
        public WorkOutLogRepository(FitnessContext context) : base(context)
        {
        }

        public override async Task<WorkoutModel?> GetByIdAsync(Guid id)
        {
            return await _context.Workouts
                .Include(w => w.User)
                .Include(w => w.WorkOutPlan)
                .FirstOrDefaultAsync(w => w.Id == id);
        }

        public override async Task<IEnumerable<WorkoutModel>> GetAllAsync()
        {
            return await _context.Workouts
                .Include(w => w.User)
                .Include(w => w.WorkOutPlan)
                .ToListAsync();
        }
    }
}