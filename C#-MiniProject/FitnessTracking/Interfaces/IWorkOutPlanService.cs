using FitnessTracking.Models.DTOs;
using FitnessTracking.Models;

namespace FitnessTracking.Interfaces
{
    public interface IWorkOutPlanService
    {
        Task<WorkOutResponeDto?> GetWorkOutPlanByIdAsync(Guid id);
        Task<IEnumerable<WorkOutResponeDto>> GetAllWorkOutPlansAsync();
        Task AddWorkOutPlanAsync(WorkOutAddRequestDto workOutPlanDto);
        Task UpdateWorkOutPlanAsync(Guid id, WorkOutPlanUpdateDto updateWorkOutPlanDto);
        Task DeleteWorkOutPlanAsync(Guid id);
        Task<PaginatedResult<WorkOutResponeDto>> GetFilteredWorkOutPlansAsync(WorkOutPlanFilterDto filter);
    }
}