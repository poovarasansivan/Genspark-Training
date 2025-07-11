using FitnessTracking.Models;
using FitnessTracking.Contexts;
using Microsoft.EntityFrameworkCore;

namespace FitnessTracking.Repositories
{
    public class UserWorkTaskRepository : Repository<UserWorkOutTask>
    {
        public UserWorkTaskRepository(FitnessContext context) : base(context)
        {
        }

        public override async Task<UserWorkOutTask?> GetByIdAsync(Guid id)
        {
            return await _context.UserWorkOutTask
                .Include(uwt => uwt.User)
                .Include(uwt => uwt.UserWorkOutPlans)
                .FirstOrDefaultAsync(uwt => uwt.Id == id);
        }

        public override async Task<IEnumerable<UserWorkOutTask>> GetAllAsync()
        {
            return await _context.UserWorkOutTask
                .Include(uwt => uwt.User)
                .Include(uwt => uwt.UserWorkOutPlans)
                .ToListAsync();
        }
    }
}