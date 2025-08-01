using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagementAPI.Enums;
using TaskManagementAPI.Models;

namespace TaskManagementAPI.Interfaces
{
    public interface ITaskItemRepository : IRepository<Guid, TaskItem>
    {
        Task<IEnumerable<TaskItem>> GetPaginatedAsync(int page, int pageSize);
        Task<int> GetTotalCountAsync();
        Task<List<TaskItem>> GetDraftsOlderThanAsync(DateTime cutoffDate);
        Task<IEnumerable<TaskItem>> GetByStatusAsync(TaskState status);
        Task<IEnumerable<TaskItem>> GetByCreatorIdAsync(Guid creatorId);
        Task<IEnumerable<TaskItem>> GetByAssignedToIdAsync(Guid assignedToId);
        Task<IEnumerable<TaskItem>> GetByStatusAndCreatorIdAsync(TaskState status, Guid creatorId);
        Task<IEnumerable<TaskItem>> GetByStatusAndAssignedToIdAsync(TaskState status, Guid assignedToId);

    }
}
