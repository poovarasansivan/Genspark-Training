using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagementAPI.DTOs.TaskItems;
using TaskManagementAPI.Enums;
using TaskManagementAPI.ApiResponses;

namespace TaskManagementAPI.Interfaces
{
    public interface ITaskService
    {
        Task<TaskItemResponseDto> CreateTaskAsync(TaskItemRequestDto dto, Guid creatorId);
        Task<TaskItemResponseDto> CreateDraftTask(TaskItemDraftRequestDto dto, Guid creatorId);
        Task<TaskItemResponseDto> UpdateTaskAsync(Guid taskId, TaskItemUpdateDto dto, Guid updaterId);
        Task DeleteTaskAsync(Guid taskId, Guid updaterId);
        Task<TaskItemResponseDto> GetTaskByIdAsync(Guid taskId);
        //Task<IEnumerable<TaskItemResponseDto>> GetAllTasksAsync(Guid requesterId);
        Task<PagedResponse<TaskItemResponseDto>> GetAllTasksAsync(Guid requesterId, int page, int pageSize,string? status = null, string? title = null, DateTime? dueDate = null);

        Task<IEnumerable<TaskItemResponseDto>> GetTasksByStatusAsync(TaskState status, Guid requesterId);
        
        Task<IEnumerable<TaskItemResponseDto>> SearchTasksAsync(Guid userId, string? title, DateTime? dueDate);

    }
}