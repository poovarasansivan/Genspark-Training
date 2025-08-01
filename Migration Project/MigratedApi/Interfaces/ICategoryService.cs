using MigratedApi.Models.Dtos;

namespace MigratedApi.Interfaces
{
    public interface ICategoryService
    {
        Task<CategoryResponseDto> GetCategoryByIdAsync(int id);
        Task<IEnumerable<CategoryResponseDto>> GetAllCategoriesAsync();
        Task<CategoryResponseDto> CreateCategoryAsync(CategoryRequestDto categoryDto);
        Task<CategoryResponseDto> UpdateCategoryAsync(int id, CategoryRequestDto categoryDto);
        Task<bool> DeleteCategoryAsync(int id);
    }
}