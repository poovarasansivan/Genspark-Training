using FitnessTracking.Models;
using FitnessTracking.Models.DTOs;

namespace FitnessTracking.Interfaces
{
    public interface IUserWorkOutTaskService
    {
        Task CreateUserWorkOutTaskAsync(WorkOutTaskAddRequestDto userWorkOutTask);
        Task<IEnumerable<WorkOutTaskResponseDto>> GetUserWorkOutTasksByUserIdAndCoachAsync(Guid userId, Guid coachId, Guid planId);
        Task<IEnumerable<WorkOutTaskResponseDto>> GetUserWorkOutTasksByUserIdAndPlanIdAsync(Guid userId, Guid planId);
        Task<WorkOutTaskResponseDto> UpdateUserWorkOutTaskAsync(WorkOutTaskUpdate userWorkOutTask);
        Task<WorkOutTaskResponseDto?> GetTaskByIdAsync(Guid id);

    }
}