using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Data;
using TaskManagementAPI.Interfaces;
using TaskManagementAPI.Models;

namespace TaskManagementAPI.Repositories
{
    public class TaskFileRepository : ITaskFileRepository
    {
        private readonly TaskManagementDbContext _context;

        public TaskFileRepository(TaskManagementDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(TaskFile file)
        {
            _context.TaskFiles.Add(file);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<TaskFile>> GetByTaskIdAsync(Guid taskId)
        {
            return await _context.TaskFiles
                .Where(f => f.TaskItemId == taskId)
                .OrderByDescending(f => f.UploadedAt)
                .ToListAsync();
        }

        public async Task<TaskFile?> GetByIdAsync(Guid id)
        {
            return await _context.TaskFiles.FindAsync(id);
        }
    }
}
