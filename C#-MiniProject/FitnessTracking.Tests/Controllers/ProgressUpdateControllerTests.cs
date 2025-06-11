using FitnessTracking.Controllers;
using FitnessTracking.Interfaces;
using FitnessTracking.Models.DTOs;
using FitnessTracking.Misc;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FitnessTracking.Controller
{
    [TestFixture]
    public class ProgressControllerTests
    {
        private Mock<IProgressService> _serviceMock;
        private ProgressController _controller;

        [SetUp]
        public void SetUp()
        {
            _serviceMock = new Mock<IProgressService>();
            _controller = new ProgressController(_serviceMock.Object);
        }

        [Test]
        public async Task GetProgressByIdAsync_ValidId_ReturnsOk()
        {
            var id = Guid.NewGuid();
            _serviceMock.Setup(s => s.GetProgressByIdAsync(id)).ReturnsAsync(new ProgressResponseDto { Id = id });

            var result = await _controller.GetProgressByIdAsync(id);

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task GetProgressByIdAsync_InvalidId_ReturnsBadRequest()
        {
            var result = await _controller.GetProgressByIdAsync(Guid.Empty);
            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task GetProgressByIdAsync_NotFound_ReturnsNotFound()
        {
            var id = Guid.NewGuid();
            _serviceMock.Setup(s => s.GetProgressByIdAsync(id)).ReturnsAsync((ProgressResponseDto?)null);

            var result = await _controller.GetProgressByIdAsync(id);

            Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task GetProgressByUserIdAndWorkOutPlanIdAsync_ValidIds_ReturnsOk()
        {
            var userId = Guid.NewGuid();
            var planId = Guid.NewGuid();
            _serviceMock.Setup(s => s.GetProgressByUserIdAndWorkOutPlanIdAsync(userId, planId)).ReturnsAsync(new ProgressResponseDto());

            var result = await _controller.GetProgressByUserIdAndWorkOutPlanIdAsync(userId, planId);

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task GetProgressByUserIdAndWorkOutPlanIdAsync_EmptyIds_ReturnsBadRequest()
        {
            var result = await _controller.GetProgressByUserIdAndWorkOutPlanIdAsync(Guid.Empty, Guid.Empty);
            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task GetProgressByUserIdAndWorkOutPlanIdAsync_NotFound_ReturnsNotFound()
        {
            var userId = Guid.NewGuid();
            var planId = Guid.NewGuid();
            _serviceMock.Setup(s => s.GetProgressByUserIdAndWorkOutPlanIdAsync(userId, planId)).ReturnsAsync((ProgressResponseDto?)null);

            var result = await _controller.GetProgressByUserIdAndWorkOutPlanIdAsync(userId, planId);
            Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task GetAllProgressAsync_Valid_ReturnsOk()
        {
            _serviceMock.Setup(s => s.GetAllProgressAsync()).ReturnsAsync(new List<ProgressResponseDto> { new ProgressResponseDto() });

            var result = await _controller.GetAllProgressAsync();
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task GetAllProgressAsync_Empty_ReturnsNotFound()
        {
            _serviceMock.Setup(s => s.GetAllProgressAsync()).ReturnsAsync(new List<ProgressResponseDto>());

            var result = await _controller.GetAllProgressAsync();
            Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task GetAllProgressAsync_ExceptionHandledGracefully()
        {
            _serviceMock.Setup(s => s.GetAllProgressAsync()).ThrowsAsync(new Exception("Service Error"));

            Assert.DoesNotThrowAsync(async () => await _controller.GetAllProgressAsync());
        }

        [Test]
        public async Task AddProgress_Valid_ReturnsOk()
        {
            var dto = new ProgressAddRequestDto { UserId = Guid.NewGuid(), Weight = 75, Date = DateTime.UtcNow };

            var result = await _controller.AddProgress(dto);

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task AddProgress_InvalidModel_ReturnsBadRequest()
        {
            _controller.ModelState.AddModelError("UserId", "Required");
            var dto = new ProgressAddRequestDto();

            var result = await _controller.AddProgress(dto);

            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public void AddProgress_Exception_ThrowsException()
        {
            _serviceMock.Setup(s => s.AddProgressAsync(It.IsAny<ProgressAddRequestDto>()))
                        .Throws(new Exception("Add failed"));

            var dto = new ProgressAddRequestDto { UserId = Guid.NewGuid(), Weight = 70, Date = DateTime.UtcNow };

            var ex = Assert.ThrowsAsync<Exception>(() => _controller.AddProgress(dto));
            Assert.That(ex.Message, Is.EqualTo("Add failed"));
        }


        [Test]
        public async Task GetUserProgressByWorkOutPlanIdAsync_Valid_ReturnsOk()
        {
            var id = Guid.NewGuid();
            _serviceMock.Setup(s => s.GetUserProgressByWorkOutPlanIdAsync(id)).ReturnsAsync(new List<ProgressResponseDto> { new ProgressResponseDto() });

            var result = await _controller.GetUserProgressByWorkOutPlanIdAsync(id);

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task GetUserProgressByWorkOutPlanIdAsync_EmptyId_ReturnsBadRequest()
        {
            var result = await _controller.GetUserProgressByWorkOutPlanIdAsync(Guid.Empty);
            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task GetUserProgressByWorkOutPlanIdAsync_NotFound_ReturnsNotFound()
        {
            var id = Guid.NewGuid();
            _serviceMock.Setup(s => s.GetUserProgressByWorkOutPlanIdAsync(id)).ReturnsAsync(new List<ProgressResponseDto>());

            var result = await _controller.GetUserProgressByWorkOutPlanIdAsync(id);

            Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
        }


        [Test]
        public async Task GetFilteredProgressAsync_Valid_ReturnsOk()
        {
            var filter = new ProgressUpdateFilterDto();
            _serviceMock.Setup(s => s.GetFilteredProgressUpdateAsync(filter))
                        .ReturnsAsync(new PaginatedResult<ProgressResponseDto> { Data = new List<ProgressResponseDto> { new ProgressResponseDto() } });

            var result = await _controller.GetFilteredProgressAsync(filter);
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task GetFilteredProgressAsync_Empty_ReturnsNotFound()
        {
            var filter = new ProgressUpdateFilterDto();
            _serviceMock.Setup(s => s.GetFilteredProgressUpdateAsync(filter))
                        .ReturnsAsync(new PaginatedResult<ProgressResponseDto> { Data = new List<ProgressResponseDto>() });

            var result = await _controller.GetFilteredProgressAsync(filter);
            Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task GetFilteredProgressAsync_ExceptionHandledGracefully()
        {
            var filter = new ProgressUpdateFilterDto();
            _serviceMock.Setup(s => s.GetFilteredProgressUpdateAsync(filter)).ThrowsAsync(new Exception());

            Assert.DoesNotThrowAsync(async () => await _controller.GetFilteredProgressAsync(filter));
        }
    }
}
