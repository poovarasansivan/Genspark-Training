using TaskManagementAPI.Models;
using TaskManagementAPI.DTOs.TaskItems;
using TaskManagementAPI.Enums;

namespace TaskManagementAPI.Mapper
{
    public class TaskItemDraftMapper
    {
        public TaskItem MapTaskItemDraftRequestDto(TaskItemDraftRequestDto dto, Guid createdById)
        {
            return new TaskItem
            {
                Title = dto.Title,
                Description = dto.Description,
                Status = dto.Status,
                DueDate = dto.DueDate,
                AssignedToId = dto.AssignedToId,
                CreatedById = createdById,
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}