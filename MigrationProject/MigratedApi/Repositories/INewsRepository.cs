using MigratedApi.Models;
using MigratedApi.Contexts;
using Microsoft.EntityFrameworkCore;

namespace MigratedApi.Repositories
{
    public class NewsRepository : Repository<News> 
    {
        public NewsRepository(ShopDbContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<News>> GetAllAsync()
        {
            return await _context.News
                .Include(n => n.User)
                .ToListAsync();
        }

        public override async Task<News> GetByIdAsync(int id)
        {
            var news = await _context.News
                .Include(n => n.User)
                .FirstOrDefaultAsync(n => n.NewsId == id);
            if (news == null)
            {
                throw new KeyNotFoundException($"News with id {id} not found.");
            }
            return news;
        }
    }
}