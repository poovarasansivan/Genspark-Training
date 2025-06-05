using HRApi.Contexts;
using HRApi.Models;
using HRApi.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRApi.Repositories
{
    public class UserRepository : Repository<string, User>
    {
        public UserRepository(DBContext context) : base(context)
        {
        }
        public override async Task<User> Get(string key)
        {
            return await _context.Users.SingleOrDefaultAsync(u => u.Name == key);
        }

        public override async Task<IEnumerable<User>> GetAll()
        {
            return await _context.Users.ToListAsync();
        }
    }
}