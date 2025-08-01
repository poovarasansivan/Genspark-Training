using MigratedApi.Models.Dtos;
using MigratedApi.Interfaces;
using MigratedApi.Misc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace MigratedApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ColorController : ControllerBase
    {
        private readonly IColorService _colorService;

        public ColorController(IColorService colorService)
        {
            _colorService = colorService;
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetColorById(int id)
        {
            try
            {
                var color = await _colorService.GetColorByIdAsync(id);
                return Ok(SuccessResponseHandler.Success(color, "Color retrieved successfully."));
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
        public async Task<IActionResult> GetAllColors()
        {
            try
            {
                var colors = await _colorService.GetAllColorsAsync();
                return Ok(SuccessResponseHandler.Success(colors, "Colors retrieved successfully."));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddColor([FromBody] ColorRequestDto color)
        {
            try
            {
                var createdColor = await _colorService.CreateColorAsync(color);
                return CreatedAtAction(nameof(GetColorById), new { id = createdColor.ColorId }, SuccessResponseHandler.Success(createdColor, "Color created successfully."));
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
        public async Task<IActionResult> DeleteColor(int id)
        {
            try
            {
                var result = await _colorService.DeleteColorAsync(id);
                if (result)
                    return Ok(SuccessResponseHandler.Success(result, "Color deleted successfully."));
                else
                    return NotFound("Color not found.");
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