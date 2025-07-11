using FitnessTracking.Models.DTOs;
using FitnessTracking.Models;
using FitnessTracking.Interfaces;
using FitnessTracking.Contexts;
using FitnessTracking.Repositories;
using FitnessTracking.Misc;
using Microsoft.EntityFrameworkCore;

namespace FitnessTracking.Services
{
    public class UserPlanService : IUserPlanService
    {
        private readonly UserPlanRepository _userPlanRepository;
        private readonly WorkOutPlanRepository _workOutPlanRepository;
        private readonly INotificationService _notificationService;

        private readonly FitnessContext _context;
        public UserPlanService(UserPlanRepository userPlanRepository,INotificationService notificationService, WorkOutPlanRepository workOutPlanRepository, FitnessContext context)
        {
            _context = context;
            _userPlanRepository = userPlanRepository;
            _notificationService = notificationService;
            _workOutPlanRepository = workOutPlanRepository;
        }

        public async Task AddUserPlanAsync(UserPlanAddRequestDto userPlanDto)
        {
            if (userPlanDto == null)
                throw new CustomeExceptionHandler("User Plan data cannot be null", 400);

            if (userPlanDto.UserId == Guid.Empty || userPlanDto.WorkOutPlanId == Guid.Empty)
                throw new CustomeExceptionHandler("User ID and WorkOut Plan ID cannot be empty or null", 400);

            var userPlan = new UserWorkOutPlanModel
            {
                UserId = userPlanDto.UserId,
                WorkOutPlanId = userPlanDto.WorkOutPlanId,
                IsCompleted = userPlanDto.IsCompleted ?? "No",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null
            };

            await _notificationService.SendNotificationAsync(userPlan.UserId.ToString(), "You have been assigned a new workout plan.", true);

            _context.UserWorkOutPlans.Add(userPlan);
            await _context.SaveChangesAsync();
        }

        public Task DeleteUserPlanAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new CustomeExceptionHandler("User Plan ID could not be empty or null", 404);

            var userPlan = _context.UserWorkOutPlans.FirstOrDefaultAsync(up => up.Id == id).Result;
            if (userPlan == null)
                throw new CustomeExceptionHandler("User Plan not found", 404);

            _context.UserWorkOutPlans.Remove(userPlan);
            return _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<UserPlanResponseDto>> GetAllUserPlansAsync()
        {
            var userPlans = await (from up in _context.UserWorkOutPlans
                                   join u in _context.Users on up.UserId equals u.Id
                                   join plan in _context.WorkOutPlans on up.WorkOutPlanId equals plan.Id
                                   join map in _context.CoachClientMaps on u.Id equals map.ClientId
                                   join coach in _context.Users on map.CoachId equals coach.Id
                                   select new UserPlanResponseDto
                                   {
                                       Id = up.Id,
                                       UserId = u.Id,
                                       UserName = u.Name,
                                       WorkOutPlanId = plan.Id,
                                       WorkOutPlanName = plan.Name,
                                       CoachId = coach.Id,
                                       CoachName = coach.Name,
                                       IsCompleted = up.IsCompleted,
                                       StartDate = plan.StartDate,
                                       EndDate = plan.EndDate,
                                       CreatedAt = up.CreatedAt,
                                       UpdatedAt = up.UpdatedAt
                                   }).ToListAsync();

            if (userPlans == null || !userPlans.Any())
                throw new CustomeExceptionHandler("No user plans found", 404);

            return userPlans;
        }

        public async Task<UserPlanResponseDto?> GetUserPlanByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new CustomeExceptionHandler("User Plan ID could not be empty or null", 404);

            var userPlan = await _context.UserWorkOutPlans
                .Include(up => up.User)
                .Include(up => up.WorkOutPlan)
                .FirstOrDefaultAsync(up => up.Id == id);
            if (userPlan == null)
                return null;
            return new UserPlanResponseDto
            {
                Id = userPlan.Id,
                UserId = userPlan.UserId,
                UserName = userPlan.User != null ? userPlan.User.Name : "Unknown User",
                WorkOutPlanId = userPlan.WorkOutPlanId,
                WorkOutPlanName = userPlan.WorkOutPlan != null ? userPlan.WorkOutPlan.Name : "No Plan",
                IsCompleted = userPlan.IsCompleted,
                CreatedAt = userPlan.CreatedAt,
                UpdatedAt = userPlan.UpdatedAt
            };
        }

        public async Task<IEnumerable<UserPlanResponseDto>> GetUserPlansByUserIdAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new CustomeExceptionHandler("User ID cannot be empty or null", 404);

            var userPlans = await _context.UserWorkOutPlans
                .Include(up => up.User)
                .Include(up => up.WorkOutPlan)
                .Where(up => up.UserId == userId)
                .Select(up => new UserPlanResponseDto
                {
                    Id = up.Id,
                    UserId = up.UserId,
                    UserName = up.User != null ? up.User.Name : "Unknown User",
                    WorkOutPlanId = up.WorkOutPlanId,
                    WorkOutPlanName = up.WorkOutPlan != null ? up.WorkOutPlan.Name : "No Plan",
                    IsCompleted = up.IsCompleted,
                    CreatedAt = up.CreatedAt,
                    UpdatedAt = up.UpdatedAt
                }).ToListAsync();

            return userPlans;
        }

        public async Task UpdateUserPlanAsync(Guid id, UserPlanUpdateDto updateUserPlanDto)
        {
            if (id == Guid.Empty)
                throw new CustomeExceptionHandler("User Plan ID could not be empty or null", 404);

            var userPlan = await _context.UserWorkOutPlans.FirstOrDefaultAsync(up => up.Id == id);
            if (userPlan == null)
                throw new CustomeExceptionHandler("User Plan not found", 404);

            userPlan.IsCompleted = updateUserPlanDto.IsCompleted;
            userPlan.UpdatedAt = updateUserPlanDto.UpdatedAt.HasValue
                ? DateTime.SpecifyKind(updateUserPlanDto.UpdatedAt.Value, DateTimeKind.Utc)
                : DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }


        public async Task<PaginatedResult<UserPlanResponseDto>> GetPaginatedUserPlansAsync(UserWorkOutPlanFilterDto filterDto)
        {
            if (filterDto == null)
                throw new CustomeExceptionHandler("Filter parameters cannot be null", 400);

            if (filterDto.PageNumber <= 0 || filterDto.PageSize <= 0)
                throw new CustomeExceptionHandler("Page number and page size must be greater than zero", 400);

            var query = _context.UserWorkOutPlans
                .Include(up => up.User)
                .Include(up => up.WorkOutPlan)
                .AsQueryable();

            if (filterDto.WorkOutPlanId.HasValue)
            {
                query = query.Where(up => up.WorkOutPlanId == filterDto.WorkOutPlanId.Value);
            }

            if (!string.IsNullOrEmpty(filterDto.IsCompleted))
            {
                query = query.Where(up => up.IsCompleted == filterDto.IsCompleted);
            }

            bool isAscending = filterDto.SortDirection.ToLower() == "asc";
            if (!string.IsNullOrEmpty(filterDto.SortBy))
            {
                switch (filterDto.SortBy.ToLower())
                {
                    case "iscompleted":
                        query = isAscending ? query.OrderBy(up => up.IsCompleted) : query.OrderByDescending(up => up.IsCompleted);
                        break;
                    case "name":
                        query = isAscending ? query.OrderBy(up => up.User.Name) : query.OrderByDescending(up => up.User.Name);
                        break;
                    case "workoutplanname":
                        query = isAscending ? query.OrderBy(up => up.WorkOutPlan.Name) : query.OrderByDescending(up => up.WorkOutPlan.Name);
                        break;
                    default:
                        query = query.OrderByDescending(up => up.CreatedAt);
                        break;
                }
            }
            else
            {
                query = query.OrderByDescending(up => up.CreatedAt);
            }

            int totalCount = await query.CountAsync();

            var userPlans = await query
                .Skip((filterDto.PageNumber - 1) * filterDto.PageSize)
                .Take(filterDto.PageSize)
                .Select(up => new UserPlanResponseDto
                {
                    Id = up.Id,
                    UserId = up.UserId,
                    UserName = up.User != null ? up.User.Name : "Unknown User",
                    WorkOutPlanId = up.WorkOutPlanId,
                    WorkOutPlanName = up.WorkOutPlan != null ? up.WorkOutPlan.Name : "No Plan",
                    IsCompleted = up.IsCompleted,
                    CreatedAt = up.CreatedAt,
                    UpdatedAt = up.UpdatedAt
                })
                .ToListAsync();

            return new PaginatedResult<UserPlanResponseDto>
            {
                Data = userPlans,
                TotalCount = totalCount,
                PageNumber = filterDto.PageNumber,
                PageSize = filterDto.PageSize
            };
        }

    }
}