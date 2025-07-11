using Microsoft.AspNetCore.Mvc;
using FitnessTracking.Interfaces;
using FitnessTracking.Models.DTOs;
using FitnessTracking.Misc;
using FitnessTracking.Models;
using Microsoft.AspNetCore.Authorization;

namespace FitnessTracking.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class WorkOutPlanController : ControllerBase
    {
        private readonly IWorkOutPlanService _workOutPlanService;
        private readonly ILogger<WorkOutPlanController> _logger;

        public WorkOutPlanController(IWorkOutPlanService workOutPlanService, ILogger<WorkOutPlanController> logger)
        {
            _workOutPlanService = workOutPlanService;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Coach, User")]
        public async Task<ActionResult<IEnumerable<WorkOutResponeDto>>> GetAllWorkOutPlansAsync()
        {
            _logger.LogInformation("Fetching all workout plans.");
            var workOutPlans = await _workOutPlanService.GetAllWorkOutPlansAsync();
            return Ok(ResponseHandler.Success(workOutPlans, "All workout plans fetched successfully"));
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, User, Coach")]
        public async Task<ActionResult<WorkOutResponeDto?>> GetWorkOutPlanByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                _logger.LogWarning("Workout plan ID is empty.");
                throw new CustomeExceptionHandler("Workout Plan ID could not be empty or null", 404);
            }

            _logger.LogInformation("Fetching workout plan with ID: {Id}", id);
            var workOutPlan = await _workOutPlanService.GetWorkOutPlanByIdAsync(id);
            if (workOutPlan == null)
            {
                _logger.LogWarning("Workout plan not found with ID: {Id}", id);
                return NotFound();
            }

            return Ok(ResponseHandler.Success(workOutPlan, "Workout plan fetched successfully"));
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Coach")]
        public async Task<ActionResult> AddWorkOutPlan([FromBody] WorkOutAddRequestDto workOutAddDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state in AddWorkOutPlan.");
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Adding new workout plan with name: {Name}", workOutAddDto.Name);
            await _workOutPlanService.AddWorkOutPlanAsync(workOutAddDto);

            return Ok(ResponseHandler.Success("Workout plan created successfully"));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin, Coach")]
        public async Task<ActionResult> DeleteWorkOutPlan(Guid id)
        {
            if (id == Guid.Empty)
            {
                _logger.LogWarning("Workout plan ID is empty for delete operation.");
                throw new CustomeExceptionHandler("Workout Plan ID could not be empty or null", 404);
            }

            _logger.LogInformation("Deleting workout plan with ID: {Id}", id);
            await _workOutPlanService.DeleteWorkOutPlanAsync(id);

            return Ok(ResponseHandler.Success("Workout plan deleted successfully"));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Coach")]
        public async Task<ActionResult> UpdateWorkOutPlan(Guid id, [FromBody] WorkOutPlanUpdateDto updateWorkOutPlanDto)
        {
            if (id == Guid.Empty)
            {
                _logger.LogWarning("Workout plan ID is empty for update operation.");
                throw new CustomeExceptionHandler("Workout Plan ID could not be empty or null", 404);
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state in UpdateWorkOutPlan.");
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Updating workout plan with ID: {Id}", id);
            await _workOutPlanService.UpdateWorkOutPlanAsync(id, updateWorkOutPlanDto);

            return Ok(ResponseHandler.Success("Workout plan updated successfully"));
        }

        [HttpGet("paginated")]
        [Authorize(Roles = "Admin, User, Coach")]
        public async Task<ActionResult<PaginatedResult<WorkOutResponeDto>>> GetWorkOutPlansByFilterAsync([FromQuery] WorkOutPlanFilterDto filter)
        {
            _logger.LogInformation("Fetching filtered workout plans with filter: {@Filter}", filter);
            var workOutPlans = await _workOutPlanService.GetFilteredWorkOutPlansAsync(filter);

            if (workOutPlans == null || workOutPlans.Data == null || !workOutPlans.Data.Any())
            {
                _logger.LogWarning("No workout plans found for given filter.");
                return NotFound(ResponseHandler.Error<IEnumerable<WorkOutResponeDto>>("No workout plans found"));
            }

            return Ok(ResponseHandler.Success(workOutPlans, "Workout plans fetched successfully"));
        }

        [HttpGet("grouped/{id}")]
        [Authorize(Roles = "Admin, User, Coach")]
        public async Task<ActionResult<IEnumerable<GroupedResponseDto>>> GetGroupedWorkPlans(Guid id)
        {
            if (id == Guid.Empty)
            {
                _logger.LogWarning("Workout plan ID is empty for grouped plans.");
                throw new CustomeExceptionHandler("Workout Plan ID could not be empty or null", 404);
            }

            _logger.LogInformation("Fetching grouped workout plans for ID: {Id}", id);
            var groupedPlans = await _workOutPlanService.GetGroupedWorkPlans(id);

            if (groupedPlans == null || !groupedPlans.Any())
            {
                _logger.LogWarning("No grouped workout plans found for ID: {Id}", id);
                return NotFound(ResponseHandler.Error<IEnumerable<GroupedResponseDto>>("No grouped workout plans found"));
            }

            return Ok(ResponseHandler.Success(groupedPlans, "Grouped workout plans fetched successfully"));
        }
        [HttpGet("grouppedcoach/{coachId}")]
        [Authorize(Roles = "Coach")]
        public async Task<ActionResult<IEnumerable<GroupedResponseDto>>> GetGroupedWorkPlansByCoachId(Guid coachId)
        {
            if (coachId == Guid.Empty)
            {
                _logger.LogWarning("Coach ID is empty for grouped plans.");
                throw new CustomeExceptionHandler("Coach ID could not be empty or null", 404);
            }

            _logger.LogInformation("Fetching grouped workout plans for Coach ID: {CoachId}", coachId);
            var groupedPlans = await _workOutPlanService.GetGroupedWorkPlansByCoachId(coachId);

            if (groupedPlans == null || !groupedPlans.Any())
            {
                _logger.LogWarning("No grouped workout plans found for Coach ID: {CoachId}", coachId);
                return NotFound(ResponseHandler.Error<IEnumerable<GroupedResponseDto>>("No grouped workout plans found"));
            }

            return Ok(ResponseHandler.Success(groupedPlans, "Grouped workout plans by coach fetched successfully"));
        }

        [HttpGet("userenrolledplan/{userId}")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<IEnumerable<GroupedResponseDto>>> GetWorkOutPlansByCoachIdAndPlanId(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                _logger.LogWarning("Coach ID or Plan ID is empty for fetching workout plans.");
                throw new CustomeExceptionHandler("Coach ID and Plan ID could not be empty or null", 404);
            }
            _logger.LogInformation("Fetching workout plans for User ID: {userId}", userId);
            var workOutPlans = await _workOutPlanService.GetWorkOutPlansByCoachIdAndPlanId(userId);
            if (workOutPlans == null || !workOutPlans.Any())
            {
                _logger.LogWarning("No workout plans found for Coach ID: {userId} ", userId);
                return NotFound(ResponseHandler.Error<IEnumerable<GroupedResponseDto>>("No workout plans found"));
            }
            return Ok(ResponseHandler.Success(workOutPlans, "Workout plans fetched successfully"));
        }
    }
}
