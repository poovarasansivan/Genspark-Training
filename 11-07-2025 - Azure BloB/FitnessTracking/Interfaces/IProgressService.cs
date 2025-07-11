using FitnessTracking.Models.DTOs;
using FitnessTracking.Models;

namespace FitnessTracking.Interfaces
{
    public interface IProgressService
    {
        Task<ProgressResponseDto?> GetProgressByIdAsync(Guid id);
        Task<List<ProgressResponseDto>> GetProgressByUserIdAndWorkOutPlanIdAsync(Guid userId, Guid workOutPlanId);
        Task<IEnumerable<ProgressResponseDto>> GetAllProgressAsync();
        Task<Guid> AddProgressAsync(ProgressAddRequestDto progress);
        Task<IEnumerable<ProgressResponseDto>> GetWorkOutProgressByUserId(Guid userId);
        Task<PaginatedResult<ProgressResponseDto>> GetFilteredProgressUpdateAsync(ProgressUpdateFilterDto progressFilterDto);
        Task<IEnumerable<ProgressResponseDto>> GetProgressByCoachIdAsync(Guid coachId);
        Task UpdateProgressAsync(Guid id, UpdateProgressDto updateProgressDto);
    }
}