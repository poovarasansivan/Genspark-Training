using FitnessTracking.Models;
using FitnessTracking.Contexts;
using FitnessTracking.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FitnessTracking.Repositories
{
    public class CoachClientMappingRepository : Repository<CoachClientMap>
    {
        public CoachClientMappingRepository(FitnessContext context) : base(context)
        {
        }

        public override Task<IEnumerable<CoachClientMap>> GetAllAsync()
        {
            return Task.FromResult(_context.Set<CoachClientMap>().AsEnumerable());
        }

        public override Task<CoachClientMap> GetByIdAsync(Guid id)
        {

            if (id == Guid.Empty)
                throw new ArgumentException("CoachClientMap ID cannot be empty or null", nameof(id));

            return _context.Set<CoachClientMap>().FindAsync(id).AsTask();
        }   
    }
}