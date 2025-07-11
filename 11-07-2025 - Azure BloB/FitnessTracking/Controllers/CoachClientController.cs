using FitnessTracking.Interfaces;
using FitnessTracking.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace FitnessTracking.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CoachClientMapController : ControllerBase
    {
        private readonly ICoachMappingService _coachMappingService;
        private readonly ILogger<CoachClientMapController> _logger;

        public CoachClientMapController(ICoachMappingService coachMappingService, ILogger<CoachClientMapController> logger)
        {
            _coachMappingService = coachMappingService;
            _logger = logger;
        }

        [HttpPost("map")]
        [Authorize(Roles = "Admin, Coach")]
        public async Task<IActionResult> AddMapping([FromBody] CoachMappingRequestDto dto)
        {
            try
            {
                var result = await _coachMappingService.AddCoachClientMappingAsync(dto);
                _logger.LogInformation("Coach-Client mapping created: {@Mapping}", result);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create Coach-Client mapping");
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin, Coach, User")]
        public async Task<IActionResult> GetAllMappings()
        {
            try
            {
                var result = await _coachMappingService.GetAllCoachClientMappingsAsync();
                _logger.LogInformation("Fetched all coach-client mappings");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all mappings");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("coach/{coachId}")]
        [Authorize(Roles = "Admin, Coach, User")]
        public async Task<IActionResult> GetClientsByCoach(Guid coachId)
        {
            try
            {
                var result = await _coachMappingService.GetClientsByCoachIdAsync(coachId);
                _logger.LogInformation("Fetched clients for coach {CoachId}", coachId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching clients for coach {CoachId}", coachId);
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("client/{clientId}")]
        [Authorize(Roles = "Admin, Coach, User")]

        public async Task<IActionResult> GetCoachesByClient(Guid clientId)
        {
            try
            {
                var result = await _coachMappingService.GetCoachesByClientIdAsync(clientId);
                _logger.LogInformation("Fetched coaches for client {ClientId}", clientId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching coaches for client {ClientId}", clientId);
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpDelete("{coachId}/{clientId}")]
        [Authorize(Roles = "Admin, Coach")]

        public async Task<IActionResult> RemoveMapping(Guid coachId, Guid clientId)
        {
            try
            {
                var result = await _coachMappingService.RemoveCoachClientMappingAsync(coachId, clientId);
                _logger.LogInformation("Removed mapping: Coach {CoachId} - Client {ClientId}", coachId, clientId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to remove Coach-Client mapping");
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
