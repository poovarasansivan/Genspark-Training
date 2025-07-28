using MigratedApi.Models.Dtos;
using MigratedApi.Contexts;
using MigratedApi.Interfaces;
using MigratedApi.Repositories;
using MigratedApi.Models;

namespace MigratedApi.Services
{
    public class ColorService : IColorService
    {
        private readonly ColorRepository _colorRepository;
        private readonly ShopDbContext _context;

        public ColorService(ColorRepository colorRepository, ShopDbContext context)
        {
            _colorRepository = colorRepository;
            _context = context;
        }

        public async Task<ColorResponseDto> CreateColorAsync(ColorRequestDto color)
        {
            if (color == null)
                throw new ArgumentNullException(nameof(ColorRequestDto));

            if (string.IsNullOrEmpty(color.ColorName))
                throw new ArgumentException("Color Name cannot be null or empty");

            var newColor = new Color
            {
                ColorId = 0,
                ColorName = color.ColorName,
            };

            await _colorRepository.AddAsync(newColor);
            await _context.SaveChangesAsync();

            return new ColorResponseDto
            {
                ColorId = newColor.ColorId,
                ColorName = newColor.ColorName,
            };
        }

        public async Task<bool> DeleteColorAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid Color ID");
            var color = await _colorRepository.GetByIdAsync(id);
            if (color == null)
                throw new KeyNotFoundException("Color not found");
            await _colorRepository.DeleteAsync(color.ColorId);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ColorResponseDto>> GetAllColorsAsync()
        {
            var colors = await _colorRepository.GetAllAsync();
            return colors.Select(c => new ColorResponseDto
            {
                ColorId = c.ColorId,
                ColorName = c.ColorName,
            });
        }

        public async Task<ColorResponseDto?> GetColorByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid Color ID");
            var color = await _colorRepository.GetByIdAsync(id);
            if (color == null)
                throw new KeyNotFoundException("Color not found");
            return new ColorResponseDto
            {
                ColorId = color.ColorId,
                ColorName = color.ColorName,
            };
        }

        public async Task<ColorResponseDto?> UpdateColorAsync(int id, ColorRequestDto color)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid Color ID");

            if (color == null)
                throw new ArgumentNullException(nameof(ColorRequestDto));

            if (string.IsNullOrEmpty(color.ColorName))
                throw new ArgumentException("Color Name cannot be null or empty");

            var existingColor = await _colorRepository.GetByIdAsync(id);
            if (existingColor == null)
                throw new KeyNotFoundException("Color not found");

            existingColor.ColorName = color.ColorName;

            await _colorRepository.UpdateAsync(existingColor);
            await _context.SaveChangesAsync();

            return new ColorResponseDto
            {
                ColorId = existingColor.ColorId,
                ColorName = existingColor.ColorName,
            };
        }
    }
}