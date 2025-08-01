using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagementAPI.ApiResponses;
using TaskManagementAPI.DTOs.StatusLog;
using TaskManagementAPI.Interfaces;

namespace TaskManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TaskStatusLogController : ControllerBase
    {
        private readonly ITaskStatusLogService _statusLogService;

        public TaskStatusLogController(ITaskStatusLogService statusLogService)
        {
            _statusLogService = statusLogService;
        }

        [HttpGet("task/{taskId}")]
        [Authorize(Roles = "Manager,TeamMember")]
        public async Task<ActionResult<ApiResponse<IEnumerable<TaskStatusLogResponseDto>>>> GetByTaskId(Guid taskId)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var role = User.FindFirstValue(ClaimTypes.Role)!;
                var logs = await _statusLogService.GetLogsByTaskIdAsync(taskId,userId, role);

                if (!logs.Any())
                {
                    return NotFound(ApiResponse<IEnumerable<TaskStatusLogResponseDto>>
                        .ErrorResponse("No logs found for this task", new Dictionary<string, List<string>>()));
                }

                var logDtos = logs.Select(log => new TaskStatusLogResponseDto
                {
                    Id = log.Id,
                    PreviousStatus = log.PreviousStatus,
                    NewStatus = log.NewStatus,
                    ChangedAt = log.ChangedAt,
                    ChangedById = log.ChangedById,
                    ChangedByName = log.ChangedBy?.FullName ?? string.Empty
                });

                return Ok(ApiResponse<IEnumerable<TaskStatusLogResponseDto>>.SuccessResponse(logDtos));
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, ApiResponse<string>.ErrorResponse(
                "Access denied: " + ex.Message,
                new Dictionary<string, List<string>> { { "Authorization", new List<string> { ex.Message } } }
            ));

            }
            catch (ArgumentException ex)
            {
                return NotFound(ApiResponse<IEnumerable<TaskStatusLogResponseDto>>.ErrorResponse(
                    ex.Message,
                    new Dictionary<string, List<string>>()));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<IEnumerable<TaskStatusLogResponseDto>>.ErrorResponse(
                    ex.Message,
                    new Dictionary<string, List<string>>()));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<IEnumerable<TaskStatusLogResponseDto>>.ErrorResponse(
                    "Unexpected error",
                    new Dictionary<string, List<string>> { { "Error", new List<string> { ex.Message } } }));
            }
        }

    }
}