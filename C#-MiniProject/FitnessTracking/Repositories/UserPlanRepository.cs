using FitnessTracking.Models;
using FitnessTracking.Contexts;
using Microsoft.EntityFrameworkCore;

namespace FitnessTracking.Repositories
{
    public class UserPlanRepository : Repository<UserWorkOutPlanModel>
    {
        public UserPlanRepository(FitnessContext context) : base(context)
        {
        }

        public override async Task<UserWorkOutPlanModel?> GetByIdAsync(Guid id)
        {
            return await _context.Set<UserWorkOutPlanModel>().FindAsync(id);

        }

        public override async Task<IEnumerable<UserWorkOutPlanModel>> GetAllAsync()
        {
            return await _context.Set<UserWorkOutPlanModel>().ToListAsync();
        }
    }
}