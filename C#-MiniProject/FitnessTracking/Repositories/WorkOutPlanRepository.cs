using FitnessTracking.Models.DTOs;
using FitnessTracking.Models;
using Microsoft.EntityFrameworkCore;
using FitnessTracking.Contexts;

namespace FitnessTracking.Repositories
{
    public class WorkOutPlanRepository : Repository<WorkOutPlanModel>
    {
        public WorkOutPlanRepository(FitnessContext context) : base(context)
        {
        }

        public override async Task<WorkOutPlanModel?> GetByIdAsync(Guid id)
        {
            return await _context.Set<WorkOutPlanModel>().FindAsync(id);
        }

        public override async Task<IEnumerable<WorkOutPlanModel>> GetAllAsync()
        {
            return await _context.Set<WorkOutPlanModel>().ToListAsync();
        }
    }
}