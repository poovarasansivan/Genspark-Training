using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Data;
using TaskManagementAPI.Interfaces;
using TaskManagementAPI.Models;

namespace TaskManagementAPI.Repositories
{
    public class UserRepository : Repository<Guid, User>, IUserRepository
    {
        public UserRepository(TaskManagementDbContext context) : base(context)
        {
        }

        public override async Task<User?> Get(Guid key)
        {
            return await _context.Users
                .Where(u => u.Id == key && !u.IsDeleted)
                .FirstOrDefaultAsync();
        }

        public override async Task<IEnumerable<User>> GetAll()
        {
            return await _context.Users
                .Where(u => !u.IsDeleted)
                .ToListAsync();
        }

        public async Task<User?> GetByEmail(string email)
        {
            return await _context.Users
                .Where(u => u.Email == email && !u.IsDeleted)
                .FirstOrDefaultAsync();
        }
    }
}
