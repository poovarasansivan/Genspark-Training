using TaskManagementAPI.Models;

namespace TaskManagementAPI.Interfaces
{
    public interface ITaskFileRepository
    {
        Task AddAsync(TaskFile file);
        Task<IEnumerable<TaskFile>> GetByTaskIdAsync(Guid taskId);
        Task<TaskFile?> GetByIdAsync(Guid id);
    }
}
