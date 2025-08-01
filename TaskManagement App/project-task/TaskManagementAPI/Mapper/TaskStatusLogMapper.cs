using TaskManagementAPI.DTOs.StatusLog;
using TaskManagementAPI.Models;

namespace TaskManagementAPI.Mapper
{
    public static class TaskStatusLogMapper
    {
        public static TaskStatusLogResponseDto ToDto(TaskStatusLog log)
        {
            return new TaskStatusLogResponseDto
            {
                Id = log.Id,
                PreviousStatus = log.PreviousStatus,
                NewStatus = log.NewStatus,
                ChangedAt = log.ChangedAt,
                ChangedById = log.ChangedById,
                ChangedByName = log.ChangedBy.FullName
            };
        }
    }
}
