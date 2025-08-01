using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagementAPI.Models;

namespace TaskManagementAPI.Interfaces
{
    public interface ITaskStatusLogService
    {
        Task<IEnumerable<TaskStatusLog>> GetLogsByTaskIdAsync(Guid taskId, Guid requesterId, string role);
    }
}
