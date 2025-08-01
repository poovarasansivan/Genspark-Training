using MigratedApi.Models;
using MigratedApi.Models.Dtos;

namespace MigratedApi.Interfaces
{
    public interface IColorService
    {
        Task<IEnumerable<ColorResponseDto>> GetAllColorsAsync();
        Task<ColorResponseDto?> GetColorByIdAsync(int id);
        Task<ColorResponseDto> CreateColorAsync(ColorRequestDto color);
        Task<ColorResponseDto?> UpdateColorAsync(int id, ColorRequestDto color);
        Task<bool> DeleteColorAsync(int id);
    }
}