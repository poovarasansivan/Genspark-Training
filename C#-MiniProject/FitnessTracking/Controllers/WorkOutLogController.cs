using Microsoft.AspNetCore.Mvc;
using FitnessTracking.Interfaces;
using FitnessTracking.Models.DTOs;
using FitnessTracking.Misc;
using Microsoft.AspNetCore.Authorization;

namespace FitnessTracking.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class WorkOutLogController : ControllerBase
    {
        private readonly IWorkOutLogService _workOutLogService;
        private readonly ILogger<WorkOutLogController> _logger;

        public WorkOutLogController(IWorkOutLogService workOutLogService, ILogger<WorkOutLogController> logger)
        {
            _workOutLogService = workOutLogService;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<WorkOutLogResponseDto>>> GetAllWorkOutLogsAsync()
        {
            _logger.LogInformation("Fetching all workout logs.");
            var workOutLogs = await _workOutLogService.GetAllWorkOutLogsAsync();

            if (workOutLogs == null || !workOutLogs.Any())
            {
                _logger.LogWarning("No workout logs found.");
                return NotFound(ResponseHandler.Error<IEnumerable<WorkOutLogResponseDto>>("No workout logs found"));
            }

            return Ok(ResponseHandler.Success(workOutLogs, "All workout logs fetched successfully"));
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, User")]
        public async Task<ActionResult<WorkOutLogResponseDto?>> GetWorkOutLogByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                _logger.LogWarning("Workout log ID is empty.");
                return BadRequest(ResponseHandler.Error<WorkOutLogResponseDto>("WorkOut Log ID cannot be empty or null"));
            }

            _logger.LogInformation("Fetching workout log with ID: {Id}", id);
            var workOutLog = await _workOutLogService.GetWorkOutLogByIdAsync(id);

            if (workOutLog == null)
            {
                _logger.LogWarning("Workout log with ID {Id} not found.", id);
                return NotFound(ResponseHandler.Error<WorkOutLogResponseDto>("Workout log not found"));
            }

            return Ok(ResponseHandler.Success(workOutLog, "Workout log fetched successfully"));
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<ActionResult> AddWorkOutLog([FromBody] WorkOutLogAddRequestDto workOutLogDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid workout log model state.");
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Adding new workout log for User ID: {UserId}", workOutLogDto.UserId);
            await _workOutLogService.AddWorkOutLogAsync(workOutLogDto);

            return Ok(ResponseHandler.Success("Workout log created successfully"));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteWorkOutLog(Guid id)
        {
            if (id == Guid.Empty)
            {
                _logger.LogWarning("Workout log ID is empty for delete request.");
                return BadRequest(ResponseHandler.Error<object>("WorkOut Log ID cannot be empty or null"));
            }

            _logger.LogInformation("Deleting workout log with ID: {Id}", id);
            await _workOutLogService.DeleteWorkOutLogAsync(id);

            return Ok(ResponseHandler.Success("Workout log deleted successfully"));
        }

        [HttpGet("user/{userId}")]
        [Authorize(Roles = "Admin, User")]
        public async Task<ActionResult<IEnumerable<WorkOutLogResponseDto>>> GetUserWorkOutLogsAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                _logger.LogWarning("User ID is empty in GetUserWorkOutLogsAsync.");
                return BadRequest(ResponseHandler.Error<IEnumerable<WorkOutLogResponseDto>>("User ID cannot be empty or null"));
            }

            _logger.LogInformation("Fetching workout logs for user ID: {UserId}", userId);
            var workOutLogs = await _workOutLogService.GetUserWorkOutLogsAsync(userId);

            if (workOutLogs == null || !workOutLogs.Any())
            {
                _logger.LogWarning("No workout logs found for user ID: {UserId}", userId);
                return NotFound(ResponseHandler.Error<IEnumerable<WorkOutLogResponseDto>>("No workout logs found for this user"));
            }

            return Ok(ResponseHandler.Success(workOutLogs, "User workout logs fetched successfully"));
        }

        [HttpGet("paginated")]
        [Authorize(Roles = "Admin, User")]
        public async Task<ActionResult<PaginatedResult<WorkOutLogResponseDto>>> GetPaginatedWorkOutLogsAsync([FromQuery] WorkOutLogFilterDto filterDto)
        {
            if (filterDto == null)
            {
                _logger.LogWarning("Filter DTO is null in GetPaginatedWorkOutLogsAsync.");
                return BadRequest(ResponseHandler.Error<PaginatedResult<WorkOutLogResponseDto>>("Filter data cannot be null"));
            }

            _logger.LogInformation("Fetching paginated workout logs with filter: {@Filter}", filterDto);
            var paginatedLogs = await _workOutLogService.GetPaginatedWorkOutLogsAsync(filterDto);

            if (paginatedLogs == null)
            {
                _logger.LogWarning("No paginated workout logs found with given filter.");
                return NotFound(ResponseHandler.Error<PaginatedResult<WorkOutLogResponseDto>>("No paginated workout logs found"));
            }

            return Ok(ResponseHandler.Success(paginatedLogs, "Paginated workout logs fetched successfully"));
        }
    }
}
