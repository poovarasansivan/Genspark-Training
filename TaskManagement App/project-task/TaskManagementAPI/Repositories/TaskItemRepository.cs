using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManagementAPI.Data;
using TaskManagementAPI.Enums;
using TaskManagementAPI.Interfaces;
using TaskManagementAPI.Models;

namespace TaskManagementAPI.Repositories
{
    public class TaskItemRepository : Repository<Guid, TaskItem>, ITaskItemRepository
    {
        public TaskItemRepository(TaskManagementDbContext context) : base(context) { }

        public override async Task<TaskItem?> Get(Guid key)
        {
            return await _context.TaskItems
                .Include(t => t.CreatedByUser)
                .Include(t => t.AssignedTo)
                .Include(t => t.UpdatedByUser)
                .Include(t => t.StatusLogs)
                .FirstOrDefaultAsync(t => t.Id == key && !t.IsDeleted);
        }

        public override async Task<IEnumerable<TaskItem>> GetAll()
        {
            return await _context.TaskItems
                .Where(t => !t.IsDeleted)
                .Include(t => t.CreatedByUser)
                .Include(t => t.AssignedTo)
                .Include(t => t.UpdatedByUser)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }


        public async Task<IEnumerable<TaskItem>> GetByStatusAsync(TaskState status)
        {
            return await _context.TaskItems
                .Where(t => t.Status == status && !t.IsDeleted)
                .Include(t => t.AssignedTo)
                .ToListAsync();
        }

        public async Task<IEnumerable<TaskItem>> GetPaginatedAsync(int page, int pageSize)
        {
            return await _context.TaskItems
                .Where(t => !t.IsDeleted)
                .Include(t => t.AssignedTo)
                .OrderByDescending(t => t.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetTotalCountAsync()
        {
            return await _context.TaskItems.CountAsync(t => !t.IsDeleted);
        }

        public async Task<IEnumerable<TaskItem>> GetByCreatorIdAsync(Guid creatorId)
        {
            return await _context.TaskItems
                .Include(t => t.AssignedTo)
                .Where(t => t.CreatedById == creatorId && !t.IsDeleted)
                .ToListAsync();
        }


        public async Task<IEnumerable<TaskItem>> GetByAssignedToIdAsync(Guid assignedToId)
        {
            return await _context.TaskItems
                .Include(t => t.AssignedTo)
                .Where(t => t.AssignedToId == assignedToId && !t.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<TaskItem>> GetByStatusAndCreatorIdAsync(TaskState status, Guid creatorId)
        {
            return await _context.TaskItems
                .Where(t => t.Status == status && t.CreatedById == creatorId && !t.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<TaskItem>> GetByStatusAndAssignedToIdAsync(TaskState status, Guid assignedToId)
        {
            return await _context.TaskItems
                .Where(t => t.Status == status && t.AssignedToId == assignedToId && !t.IsDeleted)
                .ToListAsync();
        }
        public async Task<List<TaskItem>> GetDraftsOlderThanAsync(DateTime cutoffDate)
        {
            return await _context.TaskItems
                .Where(t => t.Status == TaskState.Draft && t.CreatedAt < cutoffDate && !t.IsDeleted)
                .ToListAsync();
        }

    }
}
