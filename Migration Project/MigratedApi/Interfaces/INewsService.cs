using MigratedApi.Models;
using MigratedApi.Models.Dtos;

namespace MigratedApi.Interfaces
{
    public interface INewsService
    {
        Task<IEnumerable<NewsResponseDto>> GetAllNewsAsync();
        Task<NewsResponseDto?> GetNewsByIdAsync(int id);
        Task<NewsResponseDto> CreateNewsAsync(NewsRequestDto news);
        Task<NewsResponseDto?> UpdateNewsAsync(NewsRequestDto news);
        Task<bool> DeleteNewsAsync(int id);
    }
}