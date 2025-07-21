using ElearnApi.Models;
using ElearnApi.Context;
using Microsoft.EntityFrameworkCore;
using ElearnApi.Interface;


namespace ElearnApi.Repositories
{
    public class ManageVideoRepository : Repository<VideoModel>
    {
        public ManageVideoRepository(AppDbContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<VideoModel>> GetAllAsync()
        {
            return await _context.Videos.ToListAsync();
        }

        public override async Task<VideoModel> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Video ID must be greater than zero", nameof(id));
            }
            return await _context.Videos.FindAsync(id);
        }
    }
}