using TaskManagementAPI.Models;

public interface ITaskStatusLogRepository
{
    Task<TaskStatusLog> AddAsync(TaskStatusLog log);                     
    Task<IEnumerable<TaskStatusLog>> GetByTaskIdAsync(Guid taskId);      
}
