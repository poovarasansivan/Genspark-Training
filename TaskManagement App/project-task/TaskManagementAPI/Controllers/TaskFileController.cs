using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagementAPI.ApiResponses;
using TaskManagementAPI.DTOs.TaskFiles;
using TaskManagementAPI.Interfaces;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TaskFileController : ControllerBase
{
    private readonly ITaskFileService _taskFileService;

    public TaskFileController(ITaskFileService taskFileService)
    {
        _taskFileService = taskFileService;
    }

    [HttpPost("{taskId}/upload")]
    public async Task<IActionResult> UploadFile(Guid taskId, IFormFile file)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var fileDto = await _taskFileService.UploadFileAsync(taskId, file, userId);
            return Ok(ApiResponse<TaskFileDto>.SuccessResponse(fileDto, "File uploaded successfully."));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<TaskFileDto>.ErrorResponse(ex.Message, new Dictionary<string, List<string>>()));
        }
        catch (Exception ex)
        {
            return ErrorResponse<TaskFileDto>("Failed to upload file", ex);
        }
    }

    [HttpGet("{taskId}/files")]
    public async Task<IActionResult> GetFiles(Guid taskId)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _taskFileService.GetFilesAsync(taskId,userId);
            if (!result.Any())
            {
                return NotFound(ApiResponse<IEnumerable<TaskFileDto>>.ErrorResponse("No files found for the given task.", new Dictionary<string, List<string>>()));
            }
            return Ok(ApiResponse<IEnumerable<TaskFileDto>>.SuccessResponse(result, "Files retrieved successfully."));
        }
        catch (Exception ex)
        {
            return ErrorResponse<IEnumerable<TaskFileDto>>("Failed to fetch files", ex);
        }
    }

    [HttpGet("download/{fileId}")]
    public async Task<IActionResult> DownloadFile(Guid fileId)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var fileResult = await _taskFileService.DownloadFileAsync(fileId,userId);
            if (fileResult == null)
            {
                return NotFound(ApiResponse<string>.ErrorResponse("File not found.", new Dictionary<string, List<string>>()));
            }

            return fileResult;
        }
        catch (Exception ex)
        {
            return ErrorResponse<string>("Failed to download file", ex);
        }
    }

    private IActionResult ErrorResponse<T>(string message, Exception ex)
    {
        var errors = new Dictionary<string, List<string>>
        {
            { "Error", new List<string> { ex.Message } }
        };
        return BadRequest(ApiResponse<T>.ErrorResponse(message, errors));
    }
}
