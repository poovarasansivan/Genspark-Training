using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManagementAPI.DTOs.TaskItems;
using TaskManagementAPI.Enums;
using TaskManagementAPI.Interfaces;
using TaskManagementAPI.Models;
using TaskManagementAPI.Mapper;
using TaskManagementAPI.Hubs;
using Microsoft.AspNetCore.SignalR;
using TaskManagementAPI.ApiResponses;
using System.Collections.Generic;


namespace TaskManagementAPI.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskItemRepository _taskRepo;
        private readonly ITaskStatusLogRepository _logRepo;
        private readonly TaskItemMapper _createMapper;
        private readonly TaskItemDraftMapper _draftMapper;
        private readonly TaskItemUpdateMapper _updateMapper;

        private readonly IUserRepository _userRepository;

        private readonly IHubContext<TaskHub> _hubContext;

        public TaskService(
            ITaskItemRepository taskRepo,
            ITaskStatusLogRepository logRepo,
            TaskItemMapper createMapper,
            TaskItemUpdateMapper updateMapper,
            IUserRepository userRepository,
            TaskItemDraftMapper draftMapper,
             IHubContext<TaskHub> hubContext)
        {
            _taskRepo = taskRepo;
            _logRepo = logRepo;
            _createMapper = createMapper;
            _updateMapper = updateMapper;
            _draftMapper = draftMapper;
            _userRepository = userRepository;
            _hubContext = hubContext;
        }

        public async Task<TaskItemResponseDto> CreateTaskAsync(TaskItemRequestDto dto, Guid creatorId)
        {
            if (dto.AssignedToId == null || dto.AssignedToId == Guid.Empty)
                throw new ArgumentException("AssignedToId is required.");

            var assignedUser = await _userRepository.Get(dto.AssignedToId.Value);
            if (assignedUser == null)
                throw new ArgumentException("Assigned user does not exist.");
            if (assignedUser.Role != UserRole.TeamMember)
                throw new ArgumentException("Assigned user must have the TeamMember role.");

            if (dto.DueDate.HasValue && dto.DueDate.Value < DateTime.UtcNow)
                throw new ArgumentException("Due date cannot be in the past.");

            var task = _createMapper.MapTaskItemRequestDtoToTaskItem(dto, creatorId);
            await _taskRepo.Add(task);

            var creator = await _userRepository.Get(creatorId);

            var log = new TaskStatusLog
            {
                TaskItemId = task.Id,
                PreviousStatus = dto.Status,
                NewStatus = dto.Status,
                ChangedById = creatorId,
                ChangedAt = DateTime.UtcNow
            };
            await _logRepo.AddAsync(log);

            await _hubContext.Clients.Group($"user-{task.AssignedToId}")
                .SendAsync("TaskAssigned", new
                {
                    task.Id,
                    task.Title,
                    task.Description,
                    Status = task.Status.ToString(),
                    AssignedToName = assignedUser?.FullName ?? "N/A",
                    CreatedByName = creator?.FullName ?? "N/A"
                });

            Console.WriteLine($"[SignalR] Sending TaskCreated to user-{task.CreatedById}");
            await _hubContext.Clients.Group($"user-{task.CreatedById}")
                .SendAsync("TaskCreated", new
                {
                    task.Id,
                    task.Title,
                    task.Description,
                    Status = task.Status.ToString(),
                    AssignedToName = assignedUser?.FullName ?? "N/A",
                    CreatedByName = creator?.FullName ?? "N/A"
                });

            return ToResponseDto(task);
        }

        public async Task<TaskItemResponseDto> CreateDraftTask(TaskItemDraftRequestDto dto, Guid creatorId)
        {
            User? assignedUser = null;

            if (dto.AssignedToId.HasValue && dto.AssignedToId != Guid.Empty)
            {
                assignedUser = await _userRepository.Get(dto.AssignedToId.Value);
                if (assignedUser == null)
                    throw new ArgumentException("Assigned user does not exist.");
                if (assignedUser.Role != UserRole.TeamMember)
                    throw new ArgumentException("Assigned user must have the TeamMember role.");
            }

            var task = _draftMapper.MapTaskItemDraftRequestDto(dto, creatorId);
            task.Status = TaskState.Draft;

            await _taskRepo.Add(task);

            var log = new TaskStatusLog
            {
                TaskItemId = task.Id,
                PreviousStatus = TaskState.Draft,
                NewStatus = TaskState.Draft,
                ChangedById = creatorId,
                ChangedAt = DateTime.UtcNow
            };
            await _logRepo.AddAsync(log);

            return ToResponseDto(task);
        }


        public async Task<TaskItemResponseDto> UpdateTaskAsync(Guid taskId, TaskItemUpdateDto dto, Guid updaterId)
        {
            var task = await _taskRepo.Get(taskId) ?? throw new Exception("Task not found");
            var updater = await _userRepository.Get(updaterId) ?? throw new ArgumentException("Updater user not found.");

            var previousStatus = task.Status;
            var oldAssigneeId = task.AssignedToId;
            var wasReassigned = dto.AssignedToId.HasValue && dto.AssignedToId != task.AssignedToId;

            bool isCreator = task.CreatedById == updaterId;
            bool isAssignedUser = task.AssignedToId == updaterId;

            if (!(isCreator || isAssignedUser))
                throw new UnauthorizedAccessException("You are not allowed to update this task.");

            if (updater.Role == UserRole.TeamMember)
            {
                if (!isAssignedUser)
                    throw new UnauthorizedAccessException("Team members can only update tasks assigned to them.");

                if (dto.Title != null || dto.Description != null || dto.DueDate.HasValue || dto.AssignedToId.HasValue)
                    throw new ArgumentException("TeamMember can only update the status of a task.");

                if (dto.Status == null)
                    throw new ArgumentException("Status is required for TeamMember update.");

                if (dto.Status == previousStatus)
                    throw new ArgumentException("Cannot update to the same status.");
            }

            _updateMapper.MapTaskItemUpdateDtoToTaskItem(dto, task, updaterId);
            await _taskRepo.Update(task.Id, task);

            if (dto.Status != null && dto.Status != previousStatus)
            {
                var log = new TaskStatusLog
                {
                    TaskItemId = task.Id,
                    PreviousStatus = previousStatus,
                    NewStatus = dto.Status.Value,
                    ChangedById = updaterId
                };
                await _logRepo.AddAsync(log);
            }
            var assignedToUser = task.AssignedToId.HasValue ? await _userRepository.Get(task.AssignedToId.Value) : null;

            if (wasReassigned)
            {
                await _hubContext.Clients.Group($"user-{task.AssignedToId}")
                    .SendAsync("TaskAssigned", new
                    {
                        task.Id,
                        task.Title,
                        task.Description,
                        Status = task.Status.ToString(),
                        AssignedToName = assignedToUser?.FullName ?? "N/A",
                        CreatedByName = updater?.FullName ?? "N/A"
                    });

                await _hubContext.Clients.Group($"user-{oldAssigneeId}")
                    .SendAsync("TaskUnassigned", new
                    {
                        task.Id,
                        task.Title,
                        UnassignedByName = updater?.FullName ?? "N/A"
                    });
            }

            await _hubContext.Clients.Group($"user-{task.AssignedToId}")
                .SendAsync("TaskUpdated", new
                {
                    task.Id,
                    task.Title,
                    Status = task.Status.ToString(),
                    PreviousStatus = previousStatus.ToString(),
                    UpdatedById = updaterId,
                    UpdatedByName = updater?.FullName ?? "N/A",
                    AssignedToId = task.AssignedToId
                });

            if (task.CreatedById != task.AssignedToId)
            {
                await _hubContext.Clients.Group($"user-{task.CreatedById}")
                    .SendAsync("TaskUpdated", new
                    {
                        task.Id,
                        task.Title,
                        Status = task.Status.ToString(),
                        PreviousStatus = previousStatus.ToString(),
                        UpdatedById = updaterId,
                        UpdatedByName = updater?.FullName ?? "N/A",
                        AssignedToId = task.AssignedToId
                    });
            }

            return ToResponseDto(task);
        }

        public async Task DeleteTaskAsync(Guid taskId, Guid updaterId)
        {
            var task = await _taskRepo.Get(taskId) ?? throw new Exception("Task not found");
            var updater = await _userRepository.Get(updaterId);
            if (updater == null)
                throw new ArgumentException("Updater not found.");

            if (task.IsDeleted)
                throw new InvalidOperationException("Task is already deleted.");

            if (updater.Role != UserRole.Manager || task.CreatedById != updaterId)
                throw new UnauthorizedAccessException("Only the manager who created this task can delete it.");

            task.IsDeleted = true;
            task.UpdatedById = updaterId;
            task.UpdatedAt = DateTime.UtcNow;

            await _taskRepo.Update(task.Id, task);

            await _hubContext.Clients.Group($"user-{task.AssignedToId}")
                .SendAsync("TaskDeleted", new
                {
                    task.Id,
                    task.Title,
                    AssignedToId = task.AssignedToId,
                    DeletedById = updaterId
                });

            if (task.CreatedById != task.AssignedToId)
            {
                await _hubContext.Clients.Group($"user-{task.CreatedById}")
                    .SendAsync("TaskDeleted", new
                    {
                        task.Id,
                        task.Title,
                        AssignedToId = task.AssignedToId,
                        DeletedById = updaterId
                    });
            }

        }


        public async Task<TaskItemResponseDto> GetTaskByIdAsync(Guid taskId)
        {
            var task = await _taskRepo.Get(taskId) ?? throw new Exception("Task not found");
            return ToResponseDto(task);
        }

        public async Task<PagedResponse<TaskItemResponseDto>> GetAllTasksAsync(
    Guid requesterId, int page, int pageSize,
    string? status = null, string? title = null, DateTime? dueDate = null)
        {
            var user = await _userRepository.Get(requesterId);
            if (user == null) throw new ArgumentException("User not found");

            IEnumerable<TaskItem> tasks = user.Role == UserRole.Manager
                ? await _taskRepo.GetByCreatorIdAsync(requesterId)
                : await _taskRepo.GetByAssignedToIdAsync(requesterId);

            var now = DateTime.UtcNow;

            // Filter all tasks
            var filtered = tasks
                .Where(t => !t.IsDeleted)
                .Where(t => string.IsNullOrEmpty(status) || t.Status.ToString().Equals(status, StringComparison.OrdinalIgnoreCase))
                .Where(t => string.IsNullOrEmpty(title) || t.Title.Contains(title, StringComparison.OrdinalIgnoreCase))
                .Where(t => !dueDate.HasValue || t.DueDate?.Date == dueDate.Value.Date)
                .ToList();

            // Sort: drafts first (created within 3 days), then others
            var ordered = filtered
                .OrderByDescending(t => t.Status == TaskState.Draft && t.CreatedAt >= now.AddDays(-3))
                .ThenByDescending(t => t.Title)
                .ToList();

            var totalRecords = ordered.Count;
            var pagedData = ordered
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(ToResponseDto);

            return PagedResponse<TaskItemResponseDto>.Create(pagedData, page, pageSize, totalRecords);
        }



        public async Task<IEnumerable<TaskItemResponseDto>> GetTasksByStatusAsync(TaskState status, Guid requesterId)
        {
            var user = await _userRepository.Get(requesterId);
            if (user == null) throw new ArgumentException("User not found");

            IEnumerable<TaskItem> tasks;

            if (user.Role == UserRole.Manager)
            {
                tasks = await _taskRepo.GetByStatusAndCreatorIdAsync(status, requesterId);
            }
            else if (user.Role == UserRole.TeamMember)
            {
                tasks = await _taskRepo.GetByStatusAndAssignedToIdAsync(status, requesterId);
            }
            else
            {
                tasks = Enumerable.Empty<TaskItem>();
            }

            return tasks.Where(t => !t.IsDeleted).Select(ToResponseDto);
        }

        public async Task<IEnumerable<TaskItemResponseDto>> SearchTasksAsync(Guid userId, string? title, DateTime? dueDate)
        {
            var user = await _userRepository.Get(userId);
            if (user == null) throw new ArgumentException("User not found");

            IEnumerable<TaskItem> tasks;

            if (user.Role == UserRole.Manager)
            {
                tasks = await _taskRepo.GetByCreatorIdAsync(userId);
            }
            else if (user.Role == UserRole.TeamMember)
            {
                tasks = await _taskRepo.GetByAssignedToIdAsync(userId);
            }
            else
            {
                return Enumerable.Empty<TaskItemResponseDto>();
            }

            tasks = tasks.Where(t => !t.IsDeleted);

            if (!string.IsNullOrEmpty(title))
            {
                tasks = tasks.Where(t => t.Title.Contains(title, StringComparison.OrdinalIgnoreCase));
            }

            if (dueDate.HasValue)
            {
                tasks = tasks.Where(t => t.DueDate.HasValue && t.DueDate.Value.Date == dueDate.Value.Date);
            }

            return tasks.Select(ToResponseDto);
        }


        private TaskItemResponseDto ToResponseDto(TaskItem task)
        {
            return new TaskItemResponseDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status,
                DueDate = task.DueDate,
                CreatedById = task.CreatedById,
                UpdatedById = task.UpdatedById,
                AssignedToId = task.AssignedToId,
                CreatedAt = task.CreatedAt,
                UpdatedAt = task.UpdatedAt,
                AssignedToName = task.AssignedTo?.FullName,
            };
        }


    }
}
