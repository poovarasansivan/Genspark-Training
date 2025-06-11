using FitnessTracking.Models.DTOs;
using FitnessTracking.Models;
using FitnessTracking.Contexts;
using FitnessTracking.Interfaces;
using FitnessTracking.Repositories;
using FitnessTracking.Misc;
using Microsoft.EntityFrameworkCore;

namespace FitnessTracking.Services
{
    public class WorkOutLogService : IWorkOutLogService
    {
        private readonly FitnessContext _context;
        private readonly WorkOutLogRepository _workOutLogRepository;

        public WorkOutLogService(WorkOutLogRepository workOutLogRepository, FitnessContext context)
        {
            _context = context;
            _workOutLogRepository = workOutLogRepository;
        }

        public async Task AddWorkOutLogAsync(WorkOutLogAddRequestDto workOutLogDto)
        {
            if (workOutLogDto == null)
                throw new CustomeExceptionHandler("WorkOut Log data cannot be null", 400);

            var workOutLog = new WorkoutModel
            {
                UserId = workOutLogDto.UserId,
                WorkOutPlanId = workOutLogDto.WorkOutPlanId,
                Type = workOutLogDto.Type ?? "General",
                Date = workOutLogDto.Date,
                Duration = workOutLogDto.Duration,
                CaloriesBurned = workOutLogDto.CaloriesBurned,
                Notes = workOutLogDto.Notes ?? "No notes provided",
            };

            await _workOutLogRepository.AddAsync(workOutLog);
        }

        public Task DeleteWorkOutLogAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new CustomeExceptionHandler("WorkOut Log ID cannot be empty or null", 404);
            return _workOutLogRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<WorkOutLogResponseDto>> GetAllWorkOutLogsAsync()
        {
            var logs = await _context.Workouts
                .Include(w => w.User)
                .Include(w => w.WorkOutPlan)
                .Select(w => new WorkOutLogResponseDto
                {
                    Id = w.Id,
                    UserId = w.UserId,
                    UserName = w.User != null ? w.User.Name : "Unknown User",
                    WorkOutPlanId = w.WorkOutPlanId,
                    WorkOutPlanName = w.WorkOutPlan != null ? w.WorkOutPlan.Name : "No Plan",
                    Type = w.Type,
                    Date = w.Date,
                    Duration = w.Duration,
                    CaloriesBurned = w.CaloriesBurned,
                    Notes = w.Notes,
                    CreatedAt = w.CreatedAt,
                    UpdatedAt = w.UpdatedAt
                })
                .ToListAsync();

            if (logs == null || !logs.Any())
                throw new CustomeExceptionHandler("No workout logs found", 404);

            return logs;
        }

        public async Task<PaginatedResult<WorkOutLogResponseDto>> GetPaginatedWorkOutLogsAsync(WorkOutLogFilterDto filterDto)
        {
            if (filterDto == null)
                throw new CustomeExceptionHandler("Filter data cannot be null", 400);
            if (filterDto.PageNumber <= 0 || filterDto.PageSize <= 0)
                throw new CustomeExceptionHandler("Page number and size must be greater than zero", 400);

            var query = _context.Workouts
                            .Include(w => w.User)
                            .Include(w => w.WorkOutPlan)
                            .AsQueryable();

            if (filterDto.UserId.HasValue)
                query = query.Where(w => w.UserId == filterDto.UserId.Value);

            if (filterDto.WorkOutPlanId.HasValue)
                query = query.Where(w => w.WorkOutPlanId == filterDto.WorkOutPlanId.Value);

            bool isAscending = filterDto.SortDirection.ToLower() == "asc";
            if (!string.IsNullOrEmpty(filterDto.SortBy))
            {
                switch (filterDto.SortBy.ToLower())
                {
                    case "duration":
                        query = isAscending ? query.OrderBy(w => w.Duration) : query.OrderByDescending(w => w.Duration);
                        break;
                    case "caloriesburned":
                        query = isAscending ? query.OrderBy(w => w.CaloriesBurned) : query.OrderByDescending(w => w.CaloriesBurned);
                        break;
                    case "name":
                        query = isAscending ? query.OrderBy(w => w.User.Name) : query.OrderByDescending(w => w.User.Name);
                        break;
                    default:
                        query = query.OrderByDescending(w => w.CreatedAt); // default fallback
                        break;
                }
            }
            else
            {
                query = query.OrderByDescending(w => w.CreatedAt);
            }

            int totalCount = await query.CountAsync();

            var data = await query
                .Skip((filterDto.PageNumber - 1) * filterDto.PageSize)
                .Take(filterDto.PageSize)
                .ToListAsync();

            var result = data.Select(w => new WorkOutLogResponseDto
            {
                Id = w.Id,
                UserId = w.UserId,
                UserName = w.User != null ? w.User.Name : "Unknown User",
                WorkOutPlanId = w.WorkOutPlanId,
                WorkOutPlanName = w.WorkOutPlan != null ? w.WorkOutPlan.Name : "No Plan",
                Type = w.Type,
                Date = w.Date,
                Duration = w.Duration,
                CaloriesBurned = w.CaloriesBurned,
                Notes = w.Notes,
                CreatedAt = w.CreatedAt,
                UpdatedAt = w.UpdatedAt
            }).ToList();

            return new PaginatedResult<WorkOutLogResponseDto>
            {
                Data = result,
                TotalCount = totalCount,
                PageNumber = filterDto.PageNumber,
                PageSize = filterDto.PageSize
            };
        }


        public async Task<IEnumerable<WorkOutLogResponseDto>> GetUserWorkOutLogsAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new CustomeExceptionHandler("User ID cannot be empty or null", 404);

            var logs = await _context.Workouts
                .Include(w => w.User)
                .Include(w => w.WorkOutPlan)
                .Where(w => w.UserId == userId)
                .Select(w => new WorkOutLogResponseDto
                {
                    Id = w.Id,
                    UserId = w.UserId,
                    UserName = w.User != null ? w.User.Name : "Unknown User",
                    WorkOutPlanId = w.WorkOutPlanId,
                    WorkOutPlanName = w.WorkOutPlan != null ? w.WorkOutPlan.Name : "No Plan",
                    Type = w.Type,
                    Date = w.Date,
                    Duration = w.Duration,
                    CaloriesBurned = w.CaloriesBurned,
                    Notes = w.Notes,
                    CreatedAt = w.CreatedAt,
                    UpdatedAt = w.UpdatedAt
                })
                .ToListAsync();

            if (logs == null || !logs.Any())
                throw new CustomeExceptionHandler("No workout logs found for this user", 404);

            return logs;
        }

        public async Task<WorkOutLogResponseDto?> GetWorkOutLogByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new CustomeExceptionHandler("WorkOut Log ID cannot be empty or null", 404);

            var log = await _context.Workouts
                .Include(w => w.User)
                .Include(w => w.WorkOutPlan)
                .Where(w => w.Id == id)
                .Select(w => new WorkOutLogResponseDto
                {
                    Id = w.Id,
                    UserId = w.UserId,
                    UserName = w.User != null ? w.User.Name : "Unknown User",
                    WorkOutPlanId = w.WorkOutPlanId,
                    WorkOutPlanName = w.WorkOutPlan != null ? w.WorkOutPlan.Name : "No Plan",
                    Type = w.Type,
                    Date = w.Date,
                    Duration = w.Duration,
                    CaloriesBurned = w.CaloriesBurned,
                    Notes = w.Notes,
                    CreatedAt = w.CreatedAt,
                    UpdatedAt = w.UpdatedAt
                })
                .FirstOrDefaultAsync();

            if (log == null)
                throw new CustomeExceptionHandler("Workout log not found", 404);

            return log;
        }
    }
}