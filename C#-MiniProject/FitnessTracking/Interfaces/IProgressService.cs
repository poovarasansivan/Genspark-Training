using FitnessTracking.Models.DTOs;
using FitnessTracking.Models;

namespace FitnessTracking.Interfaces
{
    public interface IProgressService
    {
        Task<ProgressResponseDto?> GetProgressByIdAsync(Guid id);
        Task<ProgressResponseDto?> GetProgressByUserIdAndWorkOutPlanIdAsync(Guid userId, Guid workOutPlanId);
        Task<IEnumerable<ProgressResponseDto>> GetAllProgressAsync();
        Task AddProgressAsync(ProgressAddRequestDto progress);
        Task<IEnumerable<ProgressResponseDto>> GetUserProgressByWorkOutPlanIdAsync(Guid workOutPlanId);
        Task<PaginatedResult<ProgressResponseDto>> GetFilteredProgressUpdateAsync(ProgressUpdateFilterDto progressFilterDto);
    }
}