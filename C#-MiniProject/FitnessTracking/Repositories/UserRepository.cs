using FitnessTracking.Models;
using FitnessTracking.Contexts;
using Microsoft.EntityFrameworkCore;

namespace FitnessTracking.Repositories
{
    public class UserRepository : Repository<UserModel>
    {
        public UserRepository(FitnessContext context) : base(context)
        {
        }

        public override async Task<UserModel?> GetByIdAsync(Guid id)
        {
            return await _context.Set<UserModel>().FindAsync(id);
        }

        public override async Task<IEnumerable<UserModel>> GetAllAsync()
        {
            return await _context.Set<UserModel>().ToListAsync();
        }    
    }
}