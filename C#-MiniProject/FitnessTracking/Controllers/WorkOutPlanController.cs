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
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<WorkOutResponeDto>>> GetAllWorkOutPlansAsync()
        {
            _logger.LogInformation("Fetching all workout plans.");
            var workOutPlans = await _workOutPlanService.GetAllWorkOutPlansAsync();
            return Ok(ResponseHandler.Success(workOutPlans, "All workout plans fetched successfully"));
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, User")]
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin, User")]
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
    }
}
