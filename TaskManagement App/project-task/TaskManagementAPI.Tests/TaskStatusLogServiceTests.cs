using NUnit.Framework;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManagementAPI.Interfaces;
using TaskManagementAPI.Models;
using TaskManagementAPI.Enums;
using TaskManagementAPI.Services;

namespace TaskManagementAPI.Tests.Services
{
    [TestFixture]
    public class TaskStatusLogServiceTests
    {
        private Mock<ITaskStatusLogRepository> _logRepoMock;
        private Mock<ITaskItemRepository> _taskRepoMock;
        private TaskStatusLogService _service;

        [SetUp]
        public void Setup()
        {
            _logRepoMock = new Mock<ITaskStatusLogRepository>();
            _taskRepoMock = new Mock<ITaskItemRepository>();
            _service = new TaskStatusLogService(_logRepoMock.Object, _taskRepoMock.Object);
        }

        private static IEnumerable<TestCaseData> GetValidTaskLogTestCases()
        {
            yield return new TestCaseData(
                Guid.Parse("6e2e1b9c-1111-4c2f-aaaa-2222b8d2d2d2"),
                Guid.Parse("dcb582a6-1234-4cb2-bbbb-8cc781dcbe52"),
                "Manager"
            );
        }

        [Test]
        [TestCaseSource(nameof(GetValidTaskLogTestCases))]
        public async Task GetLogsByTaskIdAsync_TestMethod(Guid taskId, Guid requesterId, string role)
        {
            // Arrange
            var task = new TaskItem
            {
                Id = taskId,
                Title = "Sample Task",
                Description = "Sample Description",
                CreatedById = requesterId,
                CreatedAt = DateTime.UtcNow,
                Status = TaskState.ToDo,
                IsDeleted = false
            };

            var logs = new List<TaskStatusLog>
            {
                new TaskStatusLog
                {
                    TaskItemId = taskId,
                    PreviousStatus = TaskState.ToDo,
                    NewStatus = TaskState.InProgress,
                    ChangedById = Guid.NewGuid(),
                    ChangedAt = DateTime.UtcNow
                },
                new TaskStatusLog
                {
                    TaskItemId = taskId,
                    PreviousStatus = TaskState.InProgress,
                    NewStatus = TaskState.Done,
                    ChangedById = Guid.NewGuid(),
                    ChangedAt = DateTime.UtcNow.AddMinutes(30)
                }
            };

            _taskRepoMock.Setup(repo => repo.Get(taskId)).ReturnsAsync(task);
            _logRepoMock.Setup(repo => repo.GetByTaskIdAsync(taskId)).ReturnsAsync(logs);

            // Act
            var result = await _service.GetLogsByTaskIdAsync(taskId, requesterId, role);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public async Task GetLogsByTaskIdAsync_TeamMemberAssigned_ReturnsLogs()
        {
            var taskId = Guid.NewGuid();
            var requesterId = Guid.NewGuid();
            var role = "TeamMember";

            var task = new TaskItem
            {
                Id = taskId,
                AssignedToId = requesterId, // must match
                CreatedById = Guid.NewGuid(),
                Title = "Task",
                Description = "Some description",
                CreatedAt = DateTime.UtcNow,
                Status = TaskState.ToDo,
                IsDeleted = false
            };

            var logs = new List<TaskStatusLog>
            {
                new TaskStatusLog
                {
                    TaskItemId = taskId,
                    NewStatus = TaskState.InProgress,
                    ChangedAt = DateTime.UtcNow,
                    ChangedById = Guid.NewGuid()
                }
            };

            _taskRepoMock.Setup(repo => repo.Get(taskId)).ReturnsAsync(task);
            _logRepoMock.Setup(repo => repo.GetByTaskIdAsync(taskId)).ReturnsAsync(logs);

            var result = await _service.GetLogsByTaskIdAsync(taskId, requesterId, role);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
        }

        [Test]
        public void GetLogsByTaskIdAsync_ManagerNotCreator_ThrowsUnauthorized()
        {
            var taskId = Guid.NewGuid();
            var requesterId = Guid.NewGuid(); // not the creator
            var role = "Manager";

            var task = new TaskItem
            {
                Id = taskId,
                CreatedById = Guid.NewGuid(), // different user
                Title = "Unauthorized Task",
                Description = "No access",
                CreatedAt = DateTime.UtcNow,
                Status = TaskState.ToDo,
                IsDeleted = false
            };

            _taskRepoMock.Setup(repo => repo.Get(taskId)).ReturnsAsync(task);

            Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            {
                await _service.GetLogsByTaskIdAsync(taskId, requesterId, role);
            });
        }

        [Test]
        public void GetLogsByTaskIdAsync_DeletedTask_ThrowsInvalidOperation()
        {
            var taskId = Guid.NewGuid();
            var requesterId = Guid.NewGuid();
            var role = "Manager";

            var task = new TaskItem
            {
                Id = taskId,
                CreatedById = requesterId,
                Title = "Deleted Task",
                Description = "This task is deleted",
                CreatedAt = DateTime.UtcNow,
                Status = TaskState.ToDo,
                IsDeleted = true
            };

            _taskRepoMock.Setup(repo => repo.Get(taskId)).ReturnsAsync(task);

            Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await _service.GetLogsByTaskIdAsync(taskId, requesterId, role);
            });
        }
    }
}
