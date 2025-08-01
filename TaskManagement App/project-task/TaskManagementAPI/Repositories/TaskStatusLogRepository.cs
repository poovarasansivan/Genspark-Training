using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManagementAPI.Data;
using TaskManagementAPI.Interfaces;
using TaskManagementAPI.Models;

namespace TaskManagementAPI.Repositories
{
    public class TaskStatusLogRepository : ITaskStatusLogRepository
    {
        private readonly TaskManagementDbContext _context;

        public TaskStatusLogRepository(TaskManagementDbContext context)
        {
            _context = context;
        }

        public async Task<TaskStatusLog> AddAsync(TaskStatusLog log)
        {
            await _context.TaskStatusLogs.AddAsync(log);
            await _context.SaveChangesAsync();
            return log;
        }

        public async Task<IEnumerable<TaskStatusLog>> GetByTaskIdAsync(Guid taskId)
        {
            return await _context.TaskStatusLogs
                .Include(log => log.ChangedBy)
                .Where(log => log.TaskItemId == taskId)
                .OrderByDescending(log => log.ChangedAt)
                .ToListAsync();
        }
    }
}
