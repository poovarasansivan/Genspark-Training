using FitnessTracking.Models;
using Microsoft.AspNetCore.Mvc;
using FitnessTracking.Interfaces;
using FitnessTracking.Models.DTOs;
using FitnessTracking.Misc;
using Microsoft.AspNetCore.Authorization;

namespace FitnessTracking.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class UserWorkOutTaskController : ControllerBase
    {
        private readonly IUserWorkOutTaskService _userWorkOutTaskService;
        private readonly ILogger<UserWorkOutTaskController> _logger;

        public UserWorkOutTaskController(IUserWorkOutTaskService userWorkOutTaskService, ILogger<UserWorkOutTaskController> logger)
        {
            _userWorkOutTaskService = userWorkOutTaskService;
            _logger = logger;
        }

        [HttpGet("userId/{userId}/coachId/{coachId}/planId/{planId}")]
        [Authorize(Roles = "Admin, Coach, User")]
        public async Task<ActionResult<IEnumerable<WorkOutTaskResponseDto>>> GetUserWorkOutTasksByUserIdAndCoachAsync(Guid userId, Guid coachId, Guid planId)
        {
            if (userId == Guid.Empty || coachId == Guid.Empty || planId == Guid.Empty)
            {
                _logger.LogWarning("User ID or Coach ID or Plan ID is empty.");
                throw new CustomeExceptionHandler("User ID and Coach ID and Plan ID cannot be empty or null", 404);
            }

            _logger.LogInformation("Fetching workout tasks for User ID: {UserId}, Coach ID: {CoachId}, Plan ID : {PlanId}", userId, coachId, planId);
            var workOutTasks = await _userWorkOutTaskService.GetUserWorkOutTasksByUserIdAndCoachAsync(userId, coachId, planId);
            return Ok(ResponseHandler.Success(workOutTasks, "Workout tasks fetched successfully"));
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Coach")]
        public async Task<ActionResult> CreateUserWorkOutTaskAsync([FromBody] WorkOutTaskAddRequestDto userWorkOutTask)
        {
            if (userWorkOutTask == null)
            {
                _logger.LogWarning("User Work Out Task is null.");
                throw new CustomeExceptionHandler("User Work Out Task cannot be null", 400);
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state in CreateUserWorkOutTaskAsync.");
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Creating new user workout task for User ID: {UserId}, Coach ID: {CoachId}, Plan ID: {PlanId}",
                userWorkOutTask.UserId, userWorkOutTask.CoachId, userWorkOutTask.PlanId);

            await _userWorkOutTaskService.CreateUserWorkOutTaskAsync(userWorkOutTask);

            return Ok(ResponseHandler.Success("User workout task created successfully"));
        }

        [HttpGet("userId/{userId}/planId/{planId}")]
        [Authorize(Roles = "Admin, User, Coach")]
        public async Task<ActionResult<IEnumerable<WorkOutTaskResponseDto>>> GetUserWorkOutTaskByIdAsync(Guid userId, Guid planId)
        {
            if (userId == Guid.Empty || planId == Guid.Empty)
            {
                _logger.LogWarning("User ID or Plan ID is empty.");
                throw new CustomeExceptionHandler("User ID and Plan ID cannot be empty or null", 404);
            }

            _logger.LogInformation("Fetching user workout task for User ID: {UserId}, Plan ID: {PlanId}", userId, planId);
            var workOutTasks = await _userWorkOutTaskService.GetUserWorkOutTasksByUserIdAndPlanIdAsync(userId, planId);

            return Ok(ResponseHandler.Success(workOutTasks, "User workout tasks fetched successfully"));
        }

        [HttpPut("mark-completed/{taskId}")]
        [Authorize(Roles = "Admin, Coach, User")]
        public async Task<ActionResult> MarkTaskAsCompletedAsync(WorkOutTaskUpdate dto)
        {
            if (dto == null)
            {
                _logger.LogWarning("WorkOutTaskUpdate DTO is null.");
                throw new CustomeExceptionHandler("WorkOutTaskUpdate DTO cannot be null", 400);
            }
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state in MarkTaskAsCompletedAsync.");
                return BadRequest(ModelState);
            }
            if (dto.Id == Guid.Empty)
            {
                _logger.LogWarning("Task ID is empty.");
                throw new CustomeExceptionHandler("Task ID cannot be empty or null", 404);
            }

            await _userWorkOutTaskService.UpdateUserWorkOutTaskAsync(dto);

            return Ok(ResponseHandler.Success("Task marked as completed successfully"));
        }

    }

}