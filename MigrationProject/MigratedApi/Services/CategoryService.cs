using MigratedApi.Models.Dtos;
using MigratedApi.Contexts;
using MigratedApi.Interfaces;
using MigratedApi.Repositories;
using MigratedApi.Models;

namespace MigratedApi.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly CategoryRepository _categoryRepository;
        private readonly ShopDbContext _context;

        public CategoryService(CategoryRepository categoryRepository, ShopDbContext context)
        {
            _categoryRepository = categoryRepository;
            _context = context;
        }

        public async Task<CategoryResponseDto> CreateCategoryAsync(CategoryRequestDto categoryDto)
        {
            if (categoryDto == null)
                throw new ArgumentNullException(nameof(CategoryRequestDto));
            
            if (string.IsNullOrEmpty(categoryDto.Name))
                throw new ArgumentException("Category Name cannot be null or empty");

            var newCategory = new Category
            {
                CategoryId = 0,
                Name = categoryDto.Name,
            };

            await _categoryRepository.AddAsync(newCategory);
            await _context.SaveChangesAsync();

            return new CategoryResponseDto
            {
                CategoryId = newCategory.CategoryId,
                Name = newCategory.Name,
            };
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
                throw new KeyNotFoundException($"Category with ID {id} not found");

            _categoryRepository.DeleteAsync(category.CategoryId);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<CategoryResponseDto>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return categories.Select(c => new CategoryResponseDto
            {
                CategoryId = c.CategoryId,
                Name = c.Name,
            });
        }

        public async Task<CategoryResponseDto> GetCategoryByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid Category ID");

            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
                throw new KeyNotFoundException($"Category with ID {id} not found");

            return new CategoryResponseDto
            {
                CategoryId = category.CategoryId,
                Name = category.Name,
            };
        }

        public async Task<CategoryResponseDto> UpdateCategoryAsync(int id, CategoryRequestDto categoryDto)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid Category ID");

            if (categoryDto == null)
                throw new ArgumentNullException(nameof(CategoryRequestDto));

            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
                throw new KeyNotFoundException($"Category with ID {id} not found");

            if (string.IsNullOrEmpty(categoryDto.Name))
                throw new ArgumentException("Category Name cannot be null or empty");

            category.Name = categoryDto.Name;

            _categoryRepository.UpdateAsync(category);
            await _context.SaveChangesAsync();

            return new CategoryResponseDto
            {
                CategoryId = category.CategoryId,
                Name = category.Name,
            };
        }
    }
}