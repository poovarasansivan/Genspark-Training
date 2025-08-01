using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagementAPI.Interfaces;
using TaskManagementAPI.Models;

namespace TaskManagementAPI.Services
{
    public class TaskStatusLogService : ITaskStatusLogService
    {
        private readonly ITaskStatusLogRepository _logRepo;
        private readonly ITaskItemRepository _taskRepo;

        public TaskStatusLogService(ITaskStatusLogRepository logRepo, ITaskItemRepository taskRepo)
        {
            _logRepo = logRepo;
            _taskRepo = taskRepo;
        }

        public async Task<IEnumerable<TaskStatusLog>> GetLogsByTaskIdAsync(Guid taskId, Guid requesterId, string role)
        {
            var task = await _taskRepo.Get(taskId);
                if (task == null)
                    throw new ArgumentException("Task not found");

                if (task.IsDeleted)
                    throw new InvalidOperationException("This task is deleted");

                if (role == "Manager")
                {
                    if (task.CreatedById != requesterId)
                        throw new UnauthorizedAccessException("You can only view logs of tasks you created.");
                }
                else if (role == "TeamMember")
                {
                    if (task.AssignedToId != requesterId)
                        throw new UnauthorizedAccessException("You can only view logs of tasks assigned to you.");
                }
                else
                {
                    throw new UnauthorizedAccessException("Your role is not allowed to access this resource.");
                }

                return await _logRepo.GetByTaskIdAsync(taskId);
            }

        }
}
