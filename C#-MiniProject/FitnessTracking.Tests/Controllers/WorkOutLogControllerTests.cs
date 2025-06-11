
using FitnessTracking.Controllers;
using FitnessTracking.Interfaces;
using FitnessTracking.Models.DTOs;
using FitnessTracking.Misc;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FitnessTracking.Controller
{
    [TestFixture]
    public class WorkOutLogControllerTests
    {
        private Mock<IWorkOutLogService> _mockService;
        private WorkOutLogController _controller;

        [SetUp]
        public void Setup()
        {
            _mockService = new Mock<IWorkOutLogService>();
            _controller = new WorkOutLogController(_mockService.Object);
        }

        [Test]
        public async Task GetAllWorkOutLogsAsync_Valid_ReturnsOk()
        {
            _mockService.Setup(s => s.GetAllWorkOutLogsAsync()).ReturnsAsync(new List<WorkOutLogResponseDto> { new() });

            var result = await _controller.GetAllWorkOutLogsAsync();

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task GetAllWorkOutLogsAsync_Invalid_ReturnsNotFound()
        {
            _mockService.Setup(s => s.GetAllWorkOutLogsAsync()).ReturnsAsync(new List<WorkOutLogResponseDto>());

            var result = await _controller.GetAllWorkOutLogsAsync();

            Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public void GetAllWorkOutLogsAsync_Exception_ReturnsException()
        {
            _mockService.Setup(s => s.GetAllWorkOutLogsAsync()).ThrowsAsync(new Exception());

            Assert.ThrowsAsync<Exception>(() => _controller.GetAllWorkOutLogsAsync());
        }

        [Test]
        public async Task GetWorkOutLogByIdAsync_Valid_ReturnsOk()
        {
            _mockService.Setup(s => s.GetWorkOutLogByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new WorkOutLogResponseDto());

            var result = await _controller.GetWorkOutLogByIdAsync(Guid.NewGuid());

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task GetWorkOutLogByIdAsync_Invalid_ReturnsNotFound()
        {
            _mockService.Setup(s => s.GetWorkOutLogByIdAsync(It.IsAny<Guid>())).ReturnsAsync((WorkOutLogResponseDto)null);

            var result = await _controller.GetWorkOutLogByIdAsync(Guid.NewGuid());

            Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public void GetWorkOutLogByIdAsync_Exception_ReturnsException()
        {
            _mockService.Setup(s => s.GetWorkOutLogByIdAsync(It.IsAny<Guid>())).ThrowsAsync(new Exception());

            Assert.ThrowsAsync<Exception>(() => _controller.GetWorkOutLogByIdAsync(Guid.NewGuid()));
        }

        [Test]
        public async Task AddWorkOutLog_Valid_ReturnsOk()
        {
            var dto = new WorkOutLogAddRequestDto();
            var result = await _controller.AddWorkOutLog(dto);
            Assert.That(result,Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task AddWorkOutLog_Invalid_ReturnsBadRequest()
        {
            _controller.ModelState.AddModelError("error", "Invalid model");
            var result = await _controller.AddWorkOutLog(new WorkOutLogAddRequestDto());
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public void AddWorkOutLog_Exception_ReturnsException()
        {
            _mockService.Setup(s => s.AddWorkOutLogAsync(It.IsAny<WorkOutLogAddRequestDto>())).ThrowsAsync(new Exception());

            Assert.ThrowsAsync<Exception>(() => _controller.AddWorkOutLog(new WorkOutLogAddRequestDto()));
        }

        [Test]
        public async Task DeleteWorkOutLog_Valid_ReturnsOk()
        {
            var result = await _controller.DeleteWorkOutLog(Guid.NewGuid());
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task DeleteWorkOutLog_Invalid_ReturnsBadRequest()
        {
            var result = await _controller.DeleteWorkOutLog(Guid.Empty);
            Assert.That(result,Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public void DeleteWorkOutLog_Exception_ReturnsException()
        {
            _mockService.Setup(s => s.DeleteWorkOutLogAsync(It.IsAny<Guid>())).ThrowsAsync(new Exception());

            Assert.ThrowsAsync<Exception>(() => _controller.DeleteWorkOutLog(Guid.NewGuid()));
        }

        [Test]
        public async Task GetUserWorkOutLogsAsync_Valid_ReturnsOk()
        {
            _mockService.Setup(s => s.GetUserWorkOutLogsAsync(It.IsAny<Guid>())).ReturnsAsync(new List<WorkOutLogResponseDto> { new() });

            var result = await _controller.GetUserWorkOutLogsAsync(Guid.NewGuid());

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task GetUserWorkOutLogsAsync_Invalid_ReturnsNotFound()
        {
            _mockService.Setup(s => s.GetUserWorkOutLogsAsync(It.IsAny<Guid>())).ReturnsAsync(new List<WorkOutLogResponseDto>());

            var result = await _controller.GetUserWorkOutLogsAsync(Guid.NewGuid());

            Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public void GetUserWorkOutLogsAsync_Exception_ReturnsException()
        {
            _mockService.Setup(s => s.GetUserWorkOutLogsAsync(It.IsAny<Guid>())).ThrowsAsync(new Exception());

            Assert.ThrowsAsync<Exception>(() => _controller.GetUserWorkOutLogsAsync(Guid.NewGuid()));
        }

        [Test]
        public async Task GetPaginatedWorkOutLogsAsync_Valid_ReturnsOk()
        {
            _mockService.Setup(s => s.GetPaginatedWorkOutLogsAsync(It.IsAny<WorkOutLogFilterDto>())).ReturnsAsync(new PaginatedResult<WorkOutLogResponseDto>());

            var result = await _controller.GetPaginatedWorkOutLogsAsync(new WorkOutLogFilterDto());

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task GetPaginatedWorkOutLogsAsync_Invalid_ReturnsBadRequest()
        {
            var result = await _controller.GetPaginatedWorkOutLogsAsync(null);

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public void GetPaginatedWorkOutLogsAsync_Exception_ReturnsException()
        {
            _mockService.Setup(s => s.GetPaginatedWorkOutLogsAsync(It.IsAny<WorkOutLogFilterDto>())).ThrowsAsync(new Exception());

            Assert.ThrowsAsync<Exception>(() => _controller.GetPaginatedWorkOutLogsAsync(new WorkOutLogFilterDto()));
        }
    }
}
