using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagementAPI.ApiResponses;
using TaskManagementAPI.DTOs.Users;
using TaskManagementAPI.Interfaces;
using System.Security.Claims;
using TaskManagementAPI.CustomExceptions;

namespace TaskManagementAPI.Controllers
{
    [ApiController]
    [Route("api/v1/users")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: /api/v1/users?page=1&pageSize=10
        [HttpGet]
        [Authorize(Roles = "Manager")]
        public async Task<ActionResult<PagedResponse<UserResponseDto>>> GetAllUsers(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] string? role = null)
        {
            if (page <= 0 || pageSize <= 0)
            {
                return BadRequest(ApiResponse<string>.ErrorResponse(
                    "Invalid pagination parameters",
                    new Dictionary<string, List<string>> {
                        { "Pagination", new() { "Page and pageSize must be greater than 0" } }
                    }));
            }

            var response = await _userService.GetAllAsync(page, pageSize, search, role);
            if (!response.Data.Any())
            {
                response.Message = "No users found for the given filters.";
            }
            else
            {
                response.Message = "Users fetched successfully.";
            }

            return Ok(response);
        }

        // GET: /api/v1/users/{id}
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<UserResponseDto>>> GetUserById(Guid id)
        {
            var response = await _userService.GetByIdAsync(id);
            if (!response.Success)
                return NotFound(response);
            return Ok(response);
        }

        // POST: /api/v1/users
        [HttpPost]
        [Authorize(Roles ="Manager")]
        public async Task<ActionResult<ApiResponse<UserResponseDto>>> CreateUser([FromBody] UserRequestDto dto)
        {
            try
            {
                var createdById = GetUserIdFromToken();
                var response = await _userService.CreateAsync(dto, createdById);
                return CreatedAtAction(nameof(GetUserById), new { id = response.Data?.Id }, response);
            }
            catch (ConflictException ex)
            {
                return Conflict(ApiResponse<UserResponseDto>.ErrorResponse(
                    ex.Message,
                    new Dictionary<string, List<string>>()
                ));
            }
            catch (BadRequestException ex)
            {
                return BadRequest(ApiResponse<UserResponseDto>.ErrorResponse(
                    ex.Message,
                    new Dictionary<string, List<string>>()
                ));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<UserResponseDto>.ErrorResponse(
                    "An unexpected error occurred.",
                    new Dictionary<string, List<string>> { { "Error", new List<string> { ex.Message } } }
                ));
            }
        }

        // PUT: /api/v1/users/{id}
        [HttpPut("{id}")]
        [Authorize(Roles ="Manager,TeamMember")]
        public async Task<ActionResult<ApiResponse<UserResponseDto>>> UpdateUser(Guid id, [FromBody] UserUpdateDto dto)
        {
            var updatedById = GetUserIdFromToken();
            var response = await _userService.UpdateAsync(id, dto, updatedById);
            return Ok(response);
        }

        // DELETE: /api/v1/users/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Manager")]
        public async Task<ActionResult<ApiResponse<string>>> DeleteUser(Guid id)
        {
            var response = await _userService.DeleteAsync(id);
            return Ok(response);
        }

        // GET: /api/v1/users/team-members
        [HttpGet("team-members")]
        [Authorize(Roles = "Manager")]
        public async Task<ActionResult<ApiResponse<List<UserResponseDto>>>> GetAllTeamMembers()
        {
            var response = await _userService.GetAllTeamMembersAsync();
            return Ok(response);
        }


        private Guid GetUserIdFromToken()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
        }
    }
}
