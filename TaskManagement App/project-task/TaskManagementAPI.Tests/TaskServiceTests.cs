using System;
using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Moq;
using NUnit.Framework;
using TaskManagementAPI.DTOs.TaskItems;
using TaskManagementAPI.Enums;
using TaskManagementAPI.Hubs;
using TaskManagementAPI.Interfaces;
using TaskManagementAPI.Models;
using TaskManagementAPI.Services;
using TaskManagementAPI.Mapper;

namespace TaskManagementAPI.Tests.Services
{
    [TestFixture]
    public class TaskServiceTests
    {
        private Mock<ITaskItemRepository> _taskRepoMock;
        private Mock<IUserRepository> _userRepositoryMock;

        private Mock<ITaskStatusLogRepository> _logRepoMock;
        private Mock<IHubContext<TaskHub>> _hubContextMock;
        private Mock<IClientProxy> _clientProxyMock;

        private TaskItemMapper _createMapper;
        private TaskItemUpdateMapper _updateMapper;

        private TaskService _service;

        [SetUp]
        public void Setup()
        {
            _taskRepoMock = new Mock<ITaskItemRepository>();
            _logRepoMock = new Mock<ITaskStatusLogRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _hubContextMock = new Mock<IHubContext<TaskHub>>();
            _clientProxyMock = new Mock<IClientProxy>();

            _hubContextMock.Setup(h => h.Clients.All).Returns(_clientProxyMock.Object);

            _createMapper = new TaskItemMapper();
            _updateMapper = new TaskItemUpdateMapper();

            _service = new TaskService(
                _taskRepoMock.Object,
                _logRepoMock.Object,
                _createMapper,
                _updateMapper,
                _userRepositoryMock.Object,
                _hubContextMock.Object
            );
        }

        [Test]
        public async Task CreateTaskAsync_ValidData_ReturnsTaskResponseDto()
        {
            // Arrange
            var creatorId = Guid.NewGuid();
            var assignedToId = Guid.NewGuid();
            var taskId = Guid.NewGuid();

            var requestDto = new TaskItemRequestDto
            {
                Title = "Test Task",
                Description = "Some description",
                DueDate = DateTime.UtcNow.AddDays(2),
                Status = TaskState.ToDo,
                AssignedToId = assignedToId
            };

            var assignedUser = new User
            {
                Id = assignedToId,
                FullName = "John",
                Email = "john@example.com",
                PasswordHash = "hashedPassword",
                Role = UserRole.TeamMember
            };

            var creator = new User
            {
                Id = creatorId,
                FullName = "Manager",
                Email = "manager@example.com",
                PasswordHash = "hashedPassword",
                Role = UserRole.Manager
            };

            _userRepositoryMock.Setup(r => r.Get(assignedToId)).ReturnsAsync(assignedUser);
            _userRepositoryMock.Setup(r => r.Get(creatorId)).ReturnsAsync(creator);
            _taskRepoMock.Setup(r => r.Add(It.IsAny<TaskItem>()))
                        .ReturnsAsync((TaskItem task) => task);
            _logRepoMock.Setup(r => r.AddAsync(It.IsAny<TaskStatusLog>()))
                        .ReturnsAsync((TaskStatusLog log) => log);


            // Act
            var result = await _service.CreateTaskAsync(requestDto, creatorId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Test Task", result.Title);

            // Since the mapper doesn't populate AssignedTo, AssignedToName will be null
            Assert.IsNull(result.AssignedToName);
        }


        [Test]
        public void CreateTaskAsync_AssignedToIdMissing_ThrowsException()
        {
            var dto = new TaskItemRequestDto
            {
                Title = "Task",
                Description = "Description",
                Status = TaskState.ToDo
            };

            Assert.ThrowsAsync<ArgumentException>(() => _service.CreateTaskAsync(dto, Guid.NewGuid()));
        }

        [Test]
        public async Task GetTaskByIdAsync_TaskExists_ReturnsResponseDto()
        {
            var taskId = Guid.NewGuid();

            var task = new TaskItem
            {
                Id = taskId,
                Title = "Task Title",
                Description = "Task Desc",
                Status = TaskState.ToDo,
                AssignedTo = new User
                {
                    Id = Guid.NewGuid(),
                    FullName = "User A",
                    Email = "usera@example.com",
                    PasswordHash = "hashed",
                    Role = UserRole.TeamMember
                },
                CreatedById = Guid.NewGuid(),
                AssignedToId = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow
            };

            _taskRepoMock.Setup(r => r.Get(taskId)).ReturnsAsync(task);

            var result = await _service.GetTaskByIdAsync(taskId);

            Assert.IsNotNull(result);
            Assert.AreEqual("Task Title", result.Title);
            Assert.AreEqual("User A", result.AssignedToName);
        }

        [Test]
        public void GetTaskByIdAsync_TaskNotFound_ThrowsException()
        {
            _taskRepoMock.Setup(r => r.Get(It.IsAny<Guid>())).ReturnsAsync((TaskItem)null!);

            Assert.ThrowsAsync<Exception>(() => _service.GetTaskByIdAsync(Guid.NewGuid()));
        }

        [Test]
        public async Task DeleteTaskAsync_ValidManager_SoftDeletesTask()
        {
            var taskId = Guid.NewGuid();
            var updaterId = Guid.NewGuid();

            var task = new TaskItem
            {
                Id = taskId,
                Title = "To Delete",
                Description = "To delete desc",
                CreatedById = updaterId,
                IsDeleted = false,
                AssignedToId = Guid.NewGuid(),
                Status = TaskState.ToDo,
                CreatedAt = DateTime.UtcNow
            };

            var user = new User
            {
                Id = updaterId,
                FullName = "Manager",
                Email = "manager@example.com",
                PasswordHash = "pwd",
                Role = UserRole.Manager
            };

            _taskRepoMock.Setup(r => r.Get(taskId)).ReturnsAsync(task);
            _userRepositoryMock.Setup(r => r.Get(updaterId)).ReturnsAsync(user);
            _taskRepoMock.Setup(r => r.Add(It.IsAny<TaskItem>())).ReturnsAsync((TaskItem task) => task);


            await _service.DeleteTaskAsync(taskId, updaterId);

            Assert.IsTrue(task.IsDeleted);
        }

        [Test]
        public void DeleteTaskAsync_NotCreatorManager_ThrowsUnauthorizedAccessException()
        {
            var creatorId = Guid.NewGuid();
            var anotherManagerId = Guid.NewGuid();

            var task = new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = "Sample",
                Description = "Sample",
                CreatedById = creatorId,
                IsDeleted = false,
                AssignedToId = Guid.NewGuid(),
                Status = TaskState.ToDo,
                CreatedAt = DateTime.UtcNow
            };

            var updater = new User
            {
                Id = anotherManagerId,
                FullName = "Other Manager",
                Email = "other@example.com",
                PasswordHash = "pwd",
                Role = UserRole.Manager
            };

            _taskRepoMock.Setup(r => r.Get(task.Id)).ReturnsAsync(task);
            _userRepositoryMock.Setup(r => r.Get(updater.Id)).ReturnsAsync(updater);

            Assert.ThrowsAsync<UnauthorizedAccessException>(() => _service.DeleteTaskAsync(task.Id, updater.Id));
        }



        [Test]
        public async Task GetAllTasksAsync_ReturnsTaskList()
        {
            var requesterId = Guid.NewGuid();

            var user = new User
            {
                Id = requesterId,
                Role = UserRole.Manager,
                FullName = "Manager User",
                Email = "manager@example.com",
                PasswordHash = "hashedpassword"
            };

            var tasks = new List<TaskItem>
            {
                new TaskItem { Id = Guid.NewGuid(), Title = "Task 1", Description = "Desc 1", IsDeleted = false },
                new TaskItem { Id = Guid.NewGuid(), Title = "Task 2", Description = "Desc 2", IsDeleted = false }
            };

            _userRepositoryMock.Setup(r => r.Get(requesterId)).ReturnsAsync(user);
            _taskRepoMock.Setup(r => r.GetByCreatorIdAsync(requesterId)).ReturnsAsync(tasks);

            var result = await _service.GetAllTasksAsync(requesterId);

            Assert.AreEqual(2, result.Count());
        }



        [Test]
        public async Task GetTasksByStatusAsync_WithValidStatus_ReturnsFilteredTasks()
        {
            var requesterId = Guid.NewGuid();

            var user = new User
            {
                Id = requesterId,
                Role = UserRole.TeamMember,
                FullName = "Team Member",
                Email = "teammember@example.com",
                PasswordHash = "hashedpassword"
            };

            var taskList = new List<TaskItem>
            {
                new TaskItem { Id = Guid.NewGuid(), Title = "T1", Description = "Desc 1", Status = TaskState.ToDo, AssignedToId = requesterId, IsDeleted = false },
                new TaskItem { Id = Guid.NewGuid(), Title = "T2", Description = "Desc 2", Status = TaskState.Done, AssignedToId = requesterId, IsDeleted = false }
            };

            _userRepositoryMock.Setup(r => r.Get(requesterId)).ReturnsAsync(user);
            _taskRepoMock.Setup(r => r.GetByStatusAndAssignedToIdAsync(TaskState.ToDo, requesterId))
                .ReturnsAsync(taskList.Where(t => t.Status == TaskState.ToDo));

            var result = await _service.GetTasksByStatusAsync(TaskState.ToDo, requesterId);

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(TaskState.ToDo, result.First().Status);
        }



    }
}
