using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TaskManagementAPI.Controllers;
using TaskManagementAPI.Interfaces;
using TaskManagementAPI.Models;
using TaskManagementAPI.ApiResponses;
using TaskManagementAPI.Enums;
using TaskManagementAPI.DTOs.StatusLog;
using Microsoft.AspNetCore.Http;

using System.Security.Claims;

namespace TaskManagementAPI.Tests.Controllers
{
    [TestFixture]
    public class TaskStatusLogControllerTests
    {
        private Mock<ITaskStatusLogService> _statusLogServiceMock;
        private TaskStatusLogController _controller;

        [SetUp]
        public void Setup()
        {
            _statusLogServiceMock = new Mock<ITaskStatusLogService>();
            _controller = new TaskStatusLogController(_statusLogServiceMock.Object);
        }

        [Test]
        public async Task GetByTaskId_LogsExist_ReturnsOkWithLogs()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, "Manager")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var user = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            var logs = new List<TaskStatusLog>
            {
                new TaskStatusLog
                {
                    Id = Guid.NewGuid(),
                    TaskItemId = taskId,
                    PreviousStatus = TaskState.ToDo,
                    NewStatus = TaskState.InProgress,
                    ChangedAt = DateTime.UtcNow,
                    ChangedById = Guid.NewGuid(),
                    ChangedBy = new User
                    {
                        Id = Guid.NewGuid(),
                        FullName = "Test User",
                        Email = "test@example.com",
                        PasswordHash = "hash"
                    }
                }
            };

            _statusLogServiceMock
                .Setup(s => s.GetLogsByTaskIdAsync(taskId, userId, "Manager"))
                .ReturnsAsync(logs);

            // Act
            var result = await _controller.GetByTaskId(taskId);
            var okResult = result.Result as OkObjectResult;

            // Assert
            Assert.IsNotNull(okResult, "Expected OkObjectResult but was null.");
            var response = okResult.Value as ApiResponse<IEnumerable<TaskStatusLogResponseDto>>;
            Assert.IsNotNull(response, "Expected ApiResponse but was null.");
            Assert.IsTrue(response.Success);
            Assert.AreEqual("Test User", response.Data.First().ChangedByName);
        }


        [Test]
        public async Task GetByTaskId_LogsDoNotExist_ReturnsNotFound()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var role = "Manager";

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, role)
            };

            var identity = new ClaimsIdentity(claims, "Test");
            var user = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            _statusLogServiceMock
                .Setup(s => s.GetLogsByTaskIdAsync(taskId, userId, role))
                .ReturnsAsync(new List<TaskStatusLog>());

            // Act
            var result = await _controller.GetByTaskId(taskId);
            var notFoundResult = result.Result as NotFoundObjectResult;

            // Assert
            Assert.IsNotNull(notFoundResult);
            var response = notFoundResult.Value as ApiResponse<IEnumerable<TaskStatusLogResponseDto>>;
            Assert.IsNotNull(response);
            Assert.IsFalse(response.Success);
            Assert.AreEqual("No logs found for this task", response.Message);
        }

    }
}
