using FitnessTracking.Models;
using FitnessTracking.Models.DTOs;
using FitnessTracking.Interfaces;
using FitnessTracking.Repositories;
using FitnessTracking.Misc;
using Microsoft.EntityFrameworkCore;
using FitnessTracking.Contexts;
using Microsoft.AspNetCore.Mvc;

namespace FitnessTracking.Services
{
    public class UserWorkOutTaskService : IUserWorkOutTaskService
    {
        private readonly UserWorkTaskRepository _userWorkOutTaskRepository;
        private readonly FitnessContext _context;

        public UserWorkOutTaskService(
            UserWorkTaskRepository userWorkOutTaskRepository,
            FitnessContext context)
        {
            _userWorkOutTaskRepository = userWorkOutTaskRepository
                ?? throw new ArgumentNullException(nameof(userWorkOutTaskRepository));
            _context = context
                ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task CreateUserWorkOutTaskAsync(WorkOutTaskAddRequestDto userWorkOutTask)
        {
            if (userWorkOutTask == null)
                throw new ArgumentNullException(nameof(userWorkOutTask), "User Work Out Task cannot be null");

            var newTask = new UserWorkOutTask
            {
                Id = Guid.NewGuid(),
                UserId = userWorkOutTask.UserId,
                CoachId = userWorkOutTask.CoachId,
                PlanId = userWorkOutTask.PlanId,
                ExerciseName = userWorkOutTask.ExerciseName,
                Description = userWorkOutTask.Description,
                Reps = userWorkOutTask.Reps,
                Sets = userWorkOutTask.Sets,
                Weight = userWorkOutTask.Weight,
                ScheduledDate = userWorkOutTask.ScheduledDate,
            };

            await _userWorkOutTaskRepository.AddAsync(newTask);
        }

        public async Task<IEnumerable<WorkOutTaskResponseDto>> GetUserWorkOutTasksByUserIdAndCoachAsync(
            Guid userId, Guid coachId, Guid planId)
        {
            if (userId == Guid.Empty || coachId == Guid.Empty || planId == Guid.Empty)
                throw new ArgumentException("User ID, Coach ID, and Plan ID must be valid GUIDs.");

            var tasks = await _context.UserWorkOutTask
                .Where(t => t.UserId == userId && t.CoachId == coachId && t.PlanId == planId)
                .Select(t => new WorkOutTaskResponseDto
                {
                    Id = t.Id,
                    UserId = t.UserId,
                    CoachId = t.CoachId,
                    PlanId = t.PlanId,
                    ExerciseName = t.ExerciseName ?? string.Empty,
                    Description = t.Description ?? string.Empty,
                    Reps = t.Reps,
                    Sets = t.Sets,
                    Weight = t.Weight,
                    ScheduledDate = t.ScheduledDate,
                    IsCompleted = t.IsCompleted,
                })
                .ToListAsync();

            return tasks;
        }

        public async Task<IEnumerable<WorkOutTaskResponseDto>> GetUserWorkOutTasksByUserIdAndPlanIdAsync(
            Guid userId, Guid planId)
        {
            if (userId == Guid.Empty || planId == Guid.Empty)
                throw new ArgumentException("User ID and Plan ID must be valid GUIDs.");

            var tasks = await _context.UserWorkOutTask
                .Where(t => t.UserId == userId && t.PlanId == planId)
                .Select(t => new WorkOutTaskResponseDto
                {
                    Id = t.Id,
                    UserId = t.UserId,
                    CoachId = t.CoachId,
                    PlanId = t.PlanId,
                    ExerciseName = t.ExerciseName ?? string.Empty,
                    Description = t.Description ?? string.Empty,
                    Reps = t.Reps,
                    Sets = t.Sets,
                    Weight = t.Weight,
                    ScheduledDate = t.ScheduledDate,
                    IsCompleted = t.IsCompleted,

                })
                .ToListAsync();

            return tasks;
        }

        public async Task<WorkOutTaskResponseDto> UpdateUserWorkOutTaskAsync(WorkOutTaskUpdate userWorkOutTask)
        {
            if (userWorkOutTask == null)
                throw new ArgumentNullException(nameof(userWorkOutTask), "User Work Out Task cannot be null");

            var existingTask = await _context.UserWorkOutTask
                .FirstOrDefaultAsync(t => t.Id == userWorkOutTask.Id);

            if (existingTask == null)
                throw new KeyNotFoundException($"User Work Out Task with ID {userWorkOutTask.Id} not found.");

            // Mark as completed
            existingTask.CompletedDate = DateTime.UtcNow;
            existingTask.IsCompleted = true;

            await _context.SaveChangesAsync();

            return new WorkOutTaskResponseDto
            {
                UserId = existingTask.UserId,
                CoachId = existingTask.CoachId,
                PlanId = existingTask.PlanId,
                ExerciseName = existingTask.ExerciseName ?? string.Empty,
                Description = existingTask.Description ?? string.Empty,
                Reps = existingTask.Reps,
                Sets = existingTask.Sets,
                Weight = existingTask.Weight,
                ScheduledDate = existingTask.ScheduledDate
            };
        }

        public async Task<WorkOutTaskResponseDto?> GetTaskByIdAsync(Guid id)
        {
            var task = await _context.UserWorkOutTask
                .Where(t => t.Id == id)
                .Select(t => new WorkOutTaskResponseDto
                {
                    UserId = t.UserId,
                    CoachId = t.CoachId,
                    PlanId = t.PlanId,
                    ExerciseName = t.ExerciseName ?? string.Empty,
                    Description = t.Description ?? string.Empty,
                    Reps = t.Reps,
                    Sets = t.Sets,
                    Weight = t.Weight,
                    ScheduledDate = t.ScheduledDate
                })
                .FirstOrDefaultAsync();

            return task;
        }

    }
}
