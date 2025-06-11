using Microsoft.AspNetCore.Mvc;
using FitnessTracking.Models;
using FitnessTracking.Interfaces;
using FitnessTracking.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using FitnessTracking.Misc;
using Microsoft.Extensions.Logging;

namespace FitnessTracking.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<UserModel?>> GetByUserId(Guid id)
        {
            _logger.LogInformation("Fetching user with ID: {UserId}", id);

            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                _logger.LogWarning("User not found with ID: {UserId}", id);
                return NotFound();
            }

            _logger.LogInformation("User fetched successfully for ID: {UserId}", id);
            return Ok(ResponseHandler.Success(user, "User fetched successfully"));
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<UserModel>>> GetAllUsersAsync()
        {
            _logger.LogInformation("Fetching all users");

            var users = await _userService.GetAllUsersAsync();
            if (users == null)
            {
                _logger.LogWarning("No users found in the database");
                return NotFound();
            }

            _logger.LogInformation("All users fetched successfully");
            return Ok(ResponseHandler.Success(users, "All users fetched successfully"));
        }

        [HttpPost]
        // [Authorize(Roles = "Admin")]
        public async Task<ActionResult> AddUser([FromBody] UserRegisterDto user)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid user model received");
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Adding new user with email: {Email}", user.Email);

            await _userService.AddUserAsync(user);

            _logger.LogInformation("User created successfully: {Email}", user.Email);
            return Ok(ResponseHandler.Success("User created successfully"));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> UpdateUser(Guid id, [FromBody] UpdateUserDto updateUserDto)
        {
            if (id == Guid.Empty)
            {
                _logger.LogWarning("UpdateUser called with empty ID");
                throw new CustomeExceptionHandler("User Id cannot be empty", 404);
            }

            _logger.LogInformation("Updating user with ID: {UserId}", id);
            await _userService.UpdateUserAsync(id, updateUserDto);
            _logger.LogInformation("User updated successfully for ID: {UserId}", id);

            return Ok(ResponseHandler.Success("User updated successfully"));
        }

        [HttpPut("password/{id}")]
        [Authorize(Roles = "Admin, User")]
        public async Task<ActionResult> UpdatePassword(Guid id, [FromBody] UpdatePasswordDto updatePasswordDto)
        {
            if (id == Guid.Empty)
            {
                _logger.LogWarning("UpdatePassword called with empty ID");
                throw new CustomeExceptionHandler("User Id cannot be empty", 404);
            }

            _logger.LogInformation("Updating password for user ID: {UserId}", id);
            await _userService.UpdatePasswordAsync(id, updatePasswordDto);
            _logger.LogInformation("Password updated successfully for user ID: {UserId}", id);

            return Ok(ResponseHandler.Success("Password updated successfully"));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteUser(Guid id)
        {
            if (id == Guid.Empty)
            {
                _logger.LogWarning("DeleteUser called with empty ID");
                throw new CustomeExceptionHandler("User Id cannot be empty", 404);
            }

            _logger.LogInformation("Deleting user with ID: {UserId}", id);
            await _userService.DeleteUserAsync(id);
            _logger.LogInformation("User deleted successfully for ID: {UserId}", id);

            return Ok(ResponseHandler.Success("User deleted successfully"));
        }

        [HttpPut("status/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> UpdateUserStatus(Guid id, [FromBody] UpdateUserStatusDto updateUserStatusDto)
        {
            if (id == Guid.Empty || updateUserStatusDto == null)
            {
                _logger.LogWarning("UpdateUserStatus called with invalid input");
                throw new CustomeExceptionHandler("User Id cannot be empty", 404);
            }

            _logger.LogInformation("Updating user status for ID: {UserId}", id);
            await _userService.UpdateUserStatusAsyn(id, updateUserStatusDto);
            _logger.LogInformation("User status updated successfully for ID: {UserId}", id);

            return Ok(ResponseHandler.Success("User status updated successfully"));
        }

        [HttpGet("paginated")]
        [Authorize(Roles = "Admin, User")]
        public async Task<ActionResult<PaginatedResult<UserModel>>> GetPaginatedAllUsersAsync([FromQuery] PaginationParameterDto paginationParameterDto)
        {
            if (paginationParameterDto == null)
            {
                _logger.LogWarning("Pagination parameters cannot be null");
                throw new CustomeExceptionHandler("Pagination parameters cannot be null", 400);
            }

            _logger.LogInformation("Fetching paginated users - Page: {Page}, PageSize: {PageSize}",
                paginationParameterDto.PageNumber, paginationParameterDto.PageSize);

            var paginatedUsers = await _userService.GetPaginatedAllUsersAsync(paginationParameterDto);

            _logger.LogInformation("Paginated users fetched successfully");
            return Ok(ResponseHandler.Success(paginatedUsers, "Paginated users fetched successfully"));
        }
    }
}
