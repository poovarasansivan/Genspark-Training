using FitnessTracking.Models;
using FitnessTracking.Models.DTOs;


namespace FitnessTracking.Interfaces
{
    public interface IProgressImageService
    {
        Task<ProgressImageModel> AddProgressImageAsync(AddProgressImageDto addProgressImageDto);
        Task<IEnumerable<ProgressImageDto>> GetAllProgressImageAsync();
        Task<ProgressImageModel?> GetByIdAsync(Guid id);
    }
}