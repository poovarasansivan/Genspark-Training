using FitnessTracking.Models;
using FitnessTracking.Models.DTOs;

namespace FitnessTracking.Interfaces
{
    public interface IUserPlanService
    {
        Task<UserPlanResponseDto?> GetUserPlanByIdAsync(Guid id);
        Task<IEnumerable<UserPlanResponseDto>> GetAllUserPlansAsync();
        Task AddUserPlanAsync(UserPlanAddRequestDto userPlanDto);
        Task UpdateUserPlanAsync(Guid id, UserPlanUpdateDto updateUserPlanDto);
        Task DeleteUserPlanAsync(Guid id);
        Task<IEnumerable<UserPlanResponseDto>> GetUserPlansByUserIdAsync(Guid userId);
        Task<PaginatedResult<UserPlanResponseDto>> GetPaginatedUserPlansAsync(UserWorkOutPlanFilterDto filterDto);
    }
}