using FitnessTracking.Interfaces;
using FitnessTracking.Models;
using FitnessTracking.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FitnessTracking.Misc;
using FitnessTracking.Helpers;

namespace FitnessTracking.Controllers
{
    [Route("api/v1/progressimage")]
    [ApiController]
    public class ProgressImageController : ControllerBase
    {
        private readonly IProgressImageService _progressImageService;
        private readonly ILogger<ProgressImageController> _logger;

        public ProgressImageController(IProgressImageService progressImageService, ILogger<ProgressImageController> logger)
        {
            _progressImageService = progressImageService;
            _logger = logger;
        }

        [HttpPost("add")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> AddProgressImage([FromForm] AddProgressImageDto dto)
        {
            if (dto.File == null || dto.File.Length == 0)
            {
                _logger.LogWarning("AddProgressImage: File was not provided or empty.");
                return BadRequest("File is required.");
            }

            var result = await _progressImageService.AddProgressImageAsync(dto);
            _logger.LogInformation("AddProgressImage: Successfully uploaded image with ID {ImageId}", result.Id);

            return CreatedAtAction(nameof(DownloadImage), new { id = result.Id }, result);
        }

        [HttpGet("download/{id}")]
        public async Task<IActionResult> DownloadImage(Guid id)
        {
            _logger.LogInformation("DownloadImage: Fetching image with ID {ImageId}", id);

            var image = await _progressImageService.GetByIdAsync(id);
            if (image == null)
            {
                _logger.LogWarning("DownloadImage: Image not found with ID {ImageId}", id);
                return NotFound("Image not found");
            }

            _logger.LogInformation("DownloadImage: Successfully retrieved image with ID {ImageId}", id);

            return File(image.ImageData, image.ContentType,
                $"ProgressImage_{id}.{ImageExtension.GetExtension(image.ContentType)}");
        }
    }
}
