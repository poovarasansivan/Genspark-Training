using FitnessTracking.Models.DTOs;
using FitnessTracking.Models;
using FitnessTracking.Contexts;
using FitnessTracking.Interfaces;
using FitnessTracking.Repositories;
using FitnessTracking.Misc;
using Microsoft.EntityFrameworkCore;

namespace FitnessTracking.Services
{
    public class WorkOutPlanService : IWorkOutPlanService
    {

        private readonly WorkOutPlanRepository _workOutPlanRepository;
        private readonly FitnessContext _context;
        public WorkOutPlanService(WorkOutPlanRepository workOutPlanRepository, FitnessContext context)
        {
            _context = context;
            _workOutPlanRepository = workOutPlanRepository;
        }
        public async Task AddWorkOutPlanAsync(WorkOutAddRequestDto workOutPlanDto)
        {
            var workOutPlan = new WorkOutPlanModel
            {
                Name = workOutPlanDto.Name ?? "Default Plan",
                Description = workOutPlanDto.Description ?? "No description provided",
                StartDate = workOutPlanDto.StartDate,
                EndDate = workOutPlanDto.EndDate,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null
            };
            await _workOutPlanRepository.AddAsync(workOutPlan);
        }

        public async Task DeleteWorkOutPlanAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new CustomeExceptionHandler("WorkOut Plan ID could not be empty or null", 404);
            await _workOutPlanRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<WorkOutResponeDto>> GetAllWorkOutPlansAsync()
        {
            var workOutPlans = await _workOutPlanRepository.GetAllAsync();
            if (workOutPlans == null || !workOutPlans.Any())
                throw new CustomeExceptionHandler("No workout plans found", 404);
            return workOutPlans.Select(plan => new WorkOutResponeDto
            {
                Id = plan.Id,
                Name = plan.Name,
                Description = plan.Description,
                StartDate = plan.StartDate,
                EndDate = plan.EndDate,
                CreatedAt = plan.CreatedAt,
                UpdatedAt = plan.UpdatedAt
            }).ToList();
        }


        public async Task<WorkOutResponeDto?> GetWorkOutPlanByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new CustomeExceptionHandler("WorkOut Plan ID could not be empty or null", 404);

            var workOutPlan = await _workOutPlanRepository.GetByIdAsync(id);
            if (workOutPlan == null)
                return null;

            return new WorkOutResponeDto
            {
                Id = workOutPlan.Id,
                Name = workOutPlan.Name,
                Description = workOutPlan.Description,
                StartDate = workOutPlan.StartDate,
                EndDate = workOutPlan.EndDate,
                CreatedAt = workOutPlan.CreatedAt,
                UpdatedAt = workOutPlan.UpdatedAt
            };
        }

        public async Task<PaginatedResult<WorkOutResponeDto>> GetFilteredWorkOutPlansAsync(WorkOutPlanFilterDto filter)
        {
            var query = _context.WorkOutPlans.AsQueryable();

            if (!string.IsNullOrEmpty(filter.SearchTerm))
            {
                query = query.Where(w => w.Name.Contains(filter.SearchTerm));
            }

            bool isAscending = filter.SortDirection.ToLower() == "asc";

            if (!string.IsNullOrEmpty(filter.SortBy))
            {
                switch (filter.SortBy.ToLower())
                {
                    case "name":
                        query = isAscending ? query.OrderBy(w => w.Name) : query.OrderByDescending(w => w.Name);
                        break;
                    case "startdate":
                        query = isAscending ? query.OrderBy(w => w.StartDate) : query.OrderByDescending(w => w.StartDate);
                        break;
                    case "enddate":
                        query = isAscending ? query.OrderBy(w => w.EndDate) : query.OrderByDescending(w => w.EndDate);
                        break;
                    default:
                        query = query.OrderByDescending(w => w.CreatedAt); // Fallback sort
                        break;
                }
            }
            else
            {
                query = query.OrderByDescending(w => w.CreatedAt); // Default sort
            }

            int totalCount = await query.CountAsync();

            var paginatedData = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            var mappedData = paginatedData.Select(plan => new WorkOutResponeDto
            {
                Id = plan.Id,
                Name = plan.Name,
                Description = plan.Description,
                StartDate = plan.StartDate,
                EndDate = plan.EndDate,
                CreatedAt = plan.CreatedAt,
                UpdatedAt = plan.UpdatedAt
            }).ToList();

            return new PaginatedResult<WorkOutResponeDto>
            {
                Data = mappedData,
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };
        }


        public Task UpdateWorkOutPlanAsync(Guid id, WorkOutPlanUpdateDto updateWorkOutPlanDto)
        {
            if (id == Guid.Empty)
                throw new CustomeExceptionHandler("WorkOut Plan ID could not be empty or null", 404);

            var workOutPlan = _workOutPlanRepository.GetByIdAsync(id).Result;
            if (workOutPlan == null)
                throw new CustomeExceptionHandler("WorkOut Plan not found", 404);

            if (updateWorkOutPlanDto.EndDate.HasValue)
            {
                if (updateWorkOutPlanDto.EndDate.Value < workOutPlan.StartDate)
                    throw new CustomeExceptionHandler("End date cannot be earlier than start date", 400);
                if (updateWorkOutPlanDto.EndDate.Value < DateTime.UtcNow)
                    throw new CustomeExceptionHandler("End date cannot be in the past", 400);
                if (updateWorkOutPlanDto.StartDate.HasValue && updateWorkOutPlanDto.EndDate.Value < updateWorkOutPlanDto.StartDate.Value)
                    throw new CustomeExceptionHandler("End date cannot be earlier than start date", 400);

                workOutPlan.StartDate = updateWorkOutPlanDto.StartDate.Value;

                workOutPlan.EndDate = updateWorkOutPlanDto.EndDate.Value;
                workOutPlan.UpdatedAt = DateTime.UtcNow;
            }

            return _workOutPlanRepository.UpdateAsync(workOutPlan);
        }
    }
}