using MigratedApi.Models;
using MigratedApi.Models.Dtos;

namespace MigratedApi.Interfaces
{
    public interface IModelService
    {
        Task<IEnumerable<ModelResponseDto>> GetAllModelsAsync();
        Task<ModelResponseDto?> GetModelByIdAsync(int id);
        Task<ModelResponseDto> CreateModelAsync(ModelRequestDto model);
        Task<ModelResponseDto?> UpdateModelAsync(int id, ModelRequestDto model);
        Task<bool> DeleteModelAsync(int id);
    }
}