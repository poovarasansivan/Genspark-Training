using MigratedApi.Models;
using MigratedApi.Models.Dtos;
using MigratedApi.Interfaces;
using MigratedApi.Misc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace MigratedApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ModelController : ControllerBase
    {
        private readonly IModelService _modelService;

        public ModelController(IModelService modelService)
        {
            _modelService = modelService;
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetModelById(int id)
        {
            try
            {
                var model = await _modelService.GetModelByIdAsync(id);
                return Ok(SuccessResponseHandler.Success(model, "Model retrieved successfully."));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllModels()
        {
            try
            {
                var models = await _modelService.GetAllModelsAsync();
                return Ok(SuccessResponseHandler.Success(models, "Models retrieved successfully."));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddModel([FromBody] ModelRequestDto model)
        {
            try
            {
                var createdModel = await _modelService.CreateModelAsync(model);
                return CreatedAtAction(nameof(GetModelById), new { id = createdModel.ModelId }, SuccessResponseHandler.Success(createdModel, "Model created successfully."));
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteModel(int id)
        {
            try
            {
                var result = await _modelService.DeleteModelAsync(id);
                if (result)
                    return Ok(SuccessResponseHandler.Success(result, "Model deleted successfully."));
                else
                    return NotFound("Model not found.");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
