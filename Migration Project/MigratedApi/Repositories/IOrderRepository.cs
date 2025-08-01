using MigratedApi.Models;
using MigratedApi.Contexts;
using Microsoft.EntityFrameworkCore;

namespace MigratedApi.Repositories
{
    public class OrderRepository : Repository<Orders>
    {
        public OrderRepository(ShopDbContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<Orders>> GetAllAsync()
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Product)
                .ToListAsync();
        }

        public override async Task<Orders?> GetByIdAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Product)
                .FirstOrDefaultAsync(o => o.Id == id);
        }
        public async Task<IEnumerable<Orders>> GetByUserIdAsync(int userId)
        {
            return await _context.Orders
                .Where(o => o.UserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Orders>> GetByStatusAsync(string status)
        {
            return await _context.Orders
                .Where(o => o.Status == status)
                .ToListAsync();
        }

        public async Task AddRangeAsync(IEnumerable<Orders> orders)
        {
            await _context.Orders.AddRangeAsync(orders);
        }
    }
}