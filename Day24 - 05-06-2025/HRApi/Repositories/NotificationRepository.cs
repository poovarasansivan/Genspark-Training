using HRApi.Models;
using HRApi.Contexts;
using Microsoft.EntityFrameworkCore;

namespace HRApi.Repositories;

public class NotificationRepository : Repository<NotificationModel>
{
    public NotificationRepository(NotifyContext context) : base(context)
    {
    }

    public override async Task<NotificationModel?> GetByIdAsync(int id)
    {
        return await _context.Set<NotificationModel>().FindAsync(id);
    }

    public override async Task<IEnumerable<NotificationModel>> GetAllAsync()
    {
        return await _context.Set<NotificationModel>().ToListAsync();
    }
}