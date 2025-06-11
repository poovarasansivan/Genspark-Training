using Microsoft.AspNetCore.Mvc;
using FitnessTracking.Services;
using FitnessTracking.Models;
using FitnessTracking.Models.DTOs;
using FitnessTracking.Interfaces;
using FitnessTracking.Misc;
using Microsoft.AspNetCore.Authorization;

namespace FitnessTracking.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class UserPlanController : ControllerBase
    {
        private readonly IUserPlanService _userPlanService;
        private readonly ILogger<UserPlanController> _logger;

        public UserPlanController(IUserPlanService userPlanService, ILogger<UserPlanController> logger)
        {
            _userPlanService = userPlanService;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<UserPlanResponseDto>>> GetAllUserPlansAsync()
        {
            _logger.LogInformation("Fetching all user plans");
            var userPlans = await _userPlanService.GetAllUserPlansAsync();

            if (userPlans == null || !userPlans.Any())
            {
                _logger.LogWarning("No user plans found");
                return NotFound(ResponseHandler.Error<IEnumerable<UserPlanResponseDto>>("No user plans found"));
            }

            return Ok(ResponseHandler.Success(userPlans, "All user plans fetched successfully"));
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, User")]
        public async Task<ActionResult<UserPlanResponseDto?>> GetUserPlanByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                _logger.LogWarning("User Plan ID is empty");
                return BadRequest(ResponseHandler.Error<UserPlanResponseDto>("User Plan ID cannot be empty or null"));
            }

            _logger.LogInformation("Fetching user plan with ID: {UserPlanId}", id);
            var userPlan = await _userPlanService.GetUserPlanByIdAsync(id);

            if (userPlan == null)
            {
                _logger.LogWarning("User plan with ID {UserPlanId} not found", id);
                return NotFound(ResponseHandler.Error<UserPlanResponseDto>("User plan not found"));
            }

            return Ok(ResponseHandler.Success(userPlan, "User plan fetched successfully"));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> AddUserPlan([FromBody] UserPlanAddRequestDto userPlanDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid user plan model state");
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Adding new user plan for user ID: {UserId}", userPlanDto.UserId);
            await _userPlanService.AddUserPlanAsync(userPlanDto);
            return Ok(ResponseHandler.Success("User plan created successfully"));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteUserPlan(Guid id)
        {
            if (id == Guid.Empty)
            {
                _logger.LogWarning("Attempted to delete user plan with empty ID");
                return BadRequest(ResponseHandler.Error<object>("User Plan ID cannot be empty or null"));
            }

            _logger.LogInformation("Deleting user plan with ID: {UserPlanId}", id);
            await _userPlanService.DeleteUserPlanAsync(id);
            return Ok(ResponseHandler.Success("User plan deleted successfully"));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> UpdateUserPlan(Guid id, [FromBody] UserPlanUpdateDto updateUserPlanDto)
        {
            if (id == Guid.Empty || updateUserPlanDto == null)
            {
                _logger.LogWarning("Invalid update request for user plan. ID: {Id}, DTO Null: {IsNull}", id, updateUserPlanDto == null);
                return BadRequest(ResponseHandler.Error<object>("User Plan ID and data cannot be empty or null"));
            }

            _logger.LogInformation("Updating user plan with ID: {UserPlanId}", id);
            await _userPlanService.UpdateUserPlanAsync(id, updateUserPlanDto);
            return Ok(ResponseHandler.Success("User plan updated successfully"));
        }

        [HttpGet("user/{userId}")]
        [Authorize(Roles = "Admin, User")]
        public async Task<ActionResult<IEnumerable<UserPlanResponseDto>>> GetUserPlansByUserIdAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                _logger.LogWarning("User ID is empty for GetUserPlansByUserId");
                return BadRequest(ResponseHandler.Error<IEnumerable<UserPlanResponseDto>>("User ID cannot be empty or null"));
            }

            _logger.LogInformation("Fetching user plans for user ID: {UserId}", userId);
            var userPlans = await _userPlanService.GetUserPlansByUserIdAsync(userId);

            if (userPlans == null || !userPlans.Any())
            {
                _logger.LogWarning("No user plans found for user ID: {UserId}", userId);
                return NotFound(ResponseHandler.Error<IEnumerable<UserPlanResponseDto>>("No user plans found for this user"));
            }

            return Ok(ResponseHandler.Success(userPlans, "User plans fetched successfully"));
        }

        [HttpGet("paginated")]
        [Authorize(Roles = "Admin, User")]
        public async Task<ActionResult<PaginatedResult<UserPlanResponseDto>>> GetPaginatedUserPlansAsync([FromQuery] UserWorkOutPlanFilterDto filterDto)
        {
            if (filterDto == null)
            {
                _logger.LogWarning("Filter data is null for paginated user plans");
                return BadRequest(ResponseHandler.Error<PaginatedResult<UserPlanResponseDto>>("Filter data cannot be null"));
            }

            _logger.LogInformation("Fetching paginated user plans with filters: {@FilterDto}", filterDto);
            var paginatedUserPlans = await _userPlanService.GetPaginatedUserPlansAsync(filterDto);

            if (paginatedUserPlans == null)
            {
                _logger.LogWarning("No paginated user plans found with the provided filter");
                return NotFound(ResponseHandler.Error<PaginatedResult<UserPlanResponseDto>>("No user plans found"));
            }

            return Ok(ResponseHandler.Success(paginatedUserPlans, "Paginated user plans fetched successfully"));
        }
    }
}
