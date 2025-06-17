using FitnessTracking.Models.DTOs;
using FitnessTracking.Models;
using FitnessTracking.Misc;

namespace FitnessTracking.Interfaces
{
    public interface ICoachMappingService
    {
        Task<CoachMappingResponseDto> AddCoachClientMappingAsync(CoachMappingRequestDto coachClientMapRequestDto);
        Task<ApiSuccessResponseDto<bool>> RemoveCoachClientMappingAsync(Guid coachId, Guid clientId);
        Task<ApiSuccessResponseDto<IEnumerable<CoachMappingResponseDto>>> GetClientsByCoachIdAsync(Guid coachId);
        Task<ApiSuccessResponseDto<IEnumerable<CoachMappingResponseDto>>> GetCoachesByClientIdAsync(Guid clientId);
        Task<ApiSuccessResponseDto<IEnumerable<CoachMappingResponseDto>>> GetAllCoachClientMappingsAsync();
    }
}