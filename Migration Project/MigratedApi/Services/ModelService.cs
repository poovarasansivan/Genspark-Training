using MigratedApi.Models.Dtos;
using MigratedApi.Contexts;
using MigratedApi.Interfaces;
using MigratedApi.Repositories;
using MigratedApi.Models;

namespace MigratedApi.Services
{
    public class ModelService : IModelService
    {
        private readonly ModelRepository _modelRepository;
        private readonly ShopDbContext _context;

        public ModelService(ModelRepository modelRepository, ShopDbContext context)
        {
            _modelRepository = modelRepository;
            _context = context;
        }

        public async Task<ModelResponseDto> CreateModelAsync(ModelRequestDto model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(ModelRequestDto));

            if (string.IsNullOrEmpty(model.ModelName))
                throw new ArgumentException("Model Name cannot be null or empty");

            var newModel = new Model
            {
                ModelId = 0,
                ModelName = model.ModelName,
            };

            await _modelRepository.AddAsync(newModel);
            await _context.SaveChangesAsync();

            return new ModelResponseDto
            {
                ModelId = newModel.ModelId,
                ModelName = newModel.ModelName,
            };
        }

        public async Task<bool> DeleteModelAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid Model ID");
            var model = await _modelRepository.GetByIdAsync(id);
            if (model == null)
                throw new KeyNotFoundException("Model not found");
            await _modelRepository.DeleteAsync(model.ModelId);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ModelResponseDto>> GetAllModelsAsync()
        {
            var models = await _modelRepository.GetAllAsync();
            return models.Select(m => new ModelResponseDto
            {
                ModelId = m.ModelId,
                ModelName = m.ModelName,
            });
        }

        public async Task<ModelResponseDto?> GetModelByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid Model ID");

            var model = await _modelRepository.GetByIdAsync(id);
            if (model == null)
                return null;

            return new ModelResponseDto
            {
                ModelId = model.ModelId,
                ModelName = model.ModelName,
            };
        }

        public async Task<ModelResponseDto?> UpdateModelAsync(int id, ModelRequestDto model)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid Model ID");

            if (model == null)
                throw new ArgumentNullException(nameof(ModelRequestDto));

            if (string.IsNullOrEmpty(model.ModelName))
                throw new ArgumentException("Model Name cannot be null or empty");

            var existingModel = await _modelRepository.GetByIdAsync(id);
            if (existingModel == null)
                return null;

            existingModel.ModelName = model.ModelName;

            await _modelRepository.UpdateAsync(existingModel);
            await _context.SaveChangesAsync();

            return new ModelResponseDto
            {
                ModelId = existingModel.ModelId,
                ModelName = existingModel.ModelName,
            };
        }
    }
}