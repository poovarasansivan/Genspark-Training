using FitnessTracking.Interfaces;
using FitnessTracking.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using FitnessTracking.Misc;
using FitnessTracking.Models;
using Microsoft.AspNetCore.Authorization;

namespace FitnessTracking.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ProgressController : ControllerBase
    {
        private readonly IProgressService _progressService;
        private readonly ILogger<ProgressController> _logger;

        public ProgressController(IProgressService progressService, ILogger<ProgressController> logger)
        {
            _progressService = progressService;
            _logger = logger;
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, User")]
        public async Task<ActionResult<ProgressResponseDto?>> GetProgressByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                _logger.LogWarning("GetProgressByIdAsync: Empty ID provided.");
                return BadRequest(ResponseHandler.Error<ProgressResponseDto>("Progress ID cannot be empty or null"));
            }

            _logger.LogInformation("GetProgressByIdAsync: Fetching progress with ID {ProgressId}", id);
            var progress = await _progressService.GetProgressByIdAsync(id);
            if (progress == null)
            {
                _logger.LogWarning("GetProgressByIdAsync: No progress found with ID {ProgressId}", id);
                return NotFound(ResponseHandler.Error<ProgressResponseDto>("Progress not found"));
            }

            _logger.LogInformation("GetProgressByIdAsync: Successfully fetched progress with ID {ProgressId}", id);
            return Ok(ResponseHandler.Success(progress, "Progress fetched successfully"));
        }

        [HttpGet("user/{userId}/workout/{workOutPlanId}")]
        [Authorize(Roles = "Admin, User")]
        public async Task<ActionResult<ProgressResponseDto?>> GetProgressByUserIdAndWorkOutPlanIdAsync(Guid userId, Guid workOutPlanId)
        {
            if (userId == Guid.Empty || workOutPlanId == Guid.Empty)
            {
                _logger.LogWarning("GetProgressByUserIdAndWorkOutPlanIdAsync: Invalid userId or workoutPlanId provided.");
                return BadRequest(ResponseHandler.Error<ProgressResponseDto>("User ID and Work Out Plan ID cannot be empty or null"));
            }

            _logger.LogInformation("GetProgressByUserIdAndWorkOutPlanIdAsync: Fetching progress for UserId {UserId} and WorkOutPlanId {PlanId}", userId, workOutPlanId);
            var progress = await _progressService.GetProgressByUserIdAndWorkOutPlanIdAsync(userId, workOutPlanId);
            if (progress == null)
            {
                _logger.LogWarning("GetProgressByUserIdAndWorkOutPlanIdAsync: No progress found for UserId {UserId} and PlanId {PlanId}", userId, workOutPlanId);
                return NotFound(ResponseHandler.Error<ProgressResponseDto>("No progress found for the specified user and workout plan"));
            }

            _logger.LogInformation("GetProgressByUserIdAndWorkOutPlanIdAsync: Progress fetched successfully.");
            return Ok(ResponseHandler.Success(progress, "User progress fetched successfully"));
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<ProgressResponseDto>>> GetAllProgressAsync()
        {
            try
            {
                _logger.LogInformation("GetAllProgressAsync: Fetching all progress records.");
                var progressList = await _progressService.GetAllProgressAsync();

                if (progressList == null || !progressList.Any())
                {
                    _logger.LogWarning("GetAllProgressAsync: No progress records found.");
                    return NotFound(ResponseHandler.Error<IEnumerable<ProgressResponseDto>>("No progress records found"));
                }

                _logger.LogInformation("GetAllProgressAsync: Successfully fetched all progress records.");
                return Ok(ResponseHandler.Success(progressList, "All progress records fetched successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAllProgressAsync: Error occurred while fetching progress.");
                return StatusCode(500, ResponseHandler.Error<IEnumerable<ProgressResponseDto>>($"An error occurred: {ex.Message}"));
            }
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<ActionResult> AddProgress([FromBody] ProgressAddRequestDto progressDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("AddProgress: Invalid model state.");
                return BadRequest(ModelState);
            }

            _logger.LogInformation("AddProgress: Adding progress for userId {UserId}", progressDto.UserId);
            await _progressService.AddProgressAsync(progressDto);
            _logger.LogInformation("AddProgress: Progress added successfully for userId {UserId}", progressDto.UserId);

            return Ok(ResponseHandler.Success("Progress added successfully"));
        }

        [HttpGet("user/workout/{workOutPlanId}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult<IEnumerable<ProgressResponseDto>>> GetUserProgressByWorkOutPlanIdAsync(Guid workOutPlanId)
        {
            if (workOutPlanId == Guid.Empty)
            {
                _logger.LogWarning("GetUserProgressByWorkOutPlanIdAsync: Empty WorkOutPlanId provided.");
                return BadRequest(ResponseHandler.Error<IEnumerable<ProgressResponseDto>>("Work Out Plan ID cannot be empty or null"));
            }

            _logger.LogInformation("GetUserProgressByWorkOutPlanIdAsync: Fetching progress for workout plan ID {WorkOutPlanId}", workOutPlanId);
            var progressList = await _progressService.GetUserProgressByWorkOutPlanIdAsync(workOutPlanId);

            if (progressList == null || !progressList.Any())
            {
                _logger.LogWarning("GetUserProgressByWorkOutPlanIdAsync: No progress records found for workout plan ID {WorkOutPlanId}", workOutPlanId);
                return NotFound(ResponseHandler.Error<IEnumerable<ProgressResponseDto>>("No progress records found for the specified workout plan"));
            }

            _logger.LogInformation("GetUserProgressByWorkOutPlanIdAsync: Successfully fetched progress list.");
            return Ok(ResponseHandler.Success(progressList, "User progress by workout plan fetched successfully"));
        }

        [HttpGet("paginated")]
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult<PaginatedResult<ProgressResponseDto>>> GetFilteredProgressAsync([FromQuery] ProgressUpdateFilterDto progressUpdateFilterDto)
        {
            try
            {
                _logger.LogInformation("GetFilteredProgressAsync: Fetching paginated progress with filters.");
                var progressUpdates = await _progressService.GetFilteredProgressUpdateAsync(progressUpdateFilterDto);

                if (progressUpdates == null || progressUpdates.Data == null || !progressUpdates.Data.Any())
                {
                    _logger.LogWarning("GetFilteredProgressAsync: No progress data found.");
                    return NotFound(ResponseHandler.Error<IEnumerable<ProgressResponseDto>>("No progress data found"));
                }

                _logger.LogInformation("GetFilteredProgressAsync: Successfully fetched filtered progress.");
                return Ok(ResponseHandler.Success(progressUpdates, "Progress data fetched successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetFilteredProgressAsync: Error while fetching filtered progress.");
                return StatusCode(500, ResponseHandler.Error<string>($"An error occurred: {ex.Message}"));
            }
        }
    }
}
