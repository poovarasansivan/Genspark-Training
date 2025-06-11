using FitnessTracking.Models.DTOs;
using FitnessTracking.Models;

namespace FitnessTracking.Interfaces
{
    public interface IWorkOutLogService
    {
        Task<WorkOutLogResponseDto?> GetWorkOutLogByIdAsync(Guid id);
        Task<IEnumerable<WorkOutLogResponseDto>> GetAllWorkOutLogsAsync();
        Task AddWorkOutLogAsync(WorkOutLogAddRequestDto workOutLogDto);
        // Task UpdateWorkOutLogAsync(Guid id, WorkOutLogUpdateDto updateWorkOutLogDto);
        Task DeleteWorkOutLogAsync(Guid id);
        Task<IEnumerable<WorkOutLogResponseDto>> GetUserWorkOutLogsAsync(Guid userId);
        Task<PaginatedResult<WorkOutLogResponseDto>> GetPaginatedWorkOutLogsAsync(WorkOutLogFilterDto filterDto);
    }
}
