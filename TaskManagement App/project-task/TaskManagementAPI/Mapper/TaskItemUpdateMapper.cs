using TaskManagementAPI.Models;
using TaskManagementAPI.DTOs.TaskItems;
using System;

namespace TaskManagementAPI.Mapper
{
    public class TaskItemUpdateMapper
    {
        public void MapTaskItemUpdateDtoToTaskItem(TaskItemUpdateDto dto, TaskItem taskItem, Guid updatedById)
        {
            if (dto.Title != null)
                taskItem.Title = dto.Title;

            if (dto.Description != null)
                taskItem.Description = dto.Description;

            if (dto.Status.HasValue)
                taskItem.Status = dto.Status.Value;

            if (dto.DueDate.HasValue)
                taskItem.DueDate = DateTime.SpecifyKind(dto.DueDate.Value, DateTimeKind.Utc);

            if (dto.AssignedToId.HasValue)
                taskItem.AssignedToId = dto.AssignedToId;

            taskItem.UpdatedById = updatedById;
            taskItem.UpdatedAt = DateTime.UtcNow;
        }
    }
}