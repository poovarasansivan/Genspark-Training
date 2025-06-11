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
    public class UserPlanControllerTests
    {
        private Mock<IUserPlanService> _serviceMock;
        private UserPlanController _controller;

        [SetUp]
        public void Setup()
        {
            _serviceMock = new Mock<IUserPlanService>();
            _controller = new UserPlanController(_serviceMock.Object);
        }

        [Test]
        public async Task GetAllUserPlansAsync_Valid_ReturnsOk()
        {
            _serviceMock.Setup(s => s.GetAllUserPlansAsync()).ReturnsAsync(new List<UserPlanResponseDto> { new() });
            var result = await _controller.GetAllUserPlansAsync();
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task GetAllUserPlansAsync_Invalid_ReturnsNotFound()
        {
            _serviceMock.Setup(s => s.GetAllUserPlansAsync()).ReturnsAsync(new List<UserPlanResponseDto>());
            var result = await _controller.GetAllUserPlansAsync();
            Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task GetAllUserPlansAsync_Exception_ReturnsStatus500()
        {
            _serviceMock.Setup(s => s.GetAllUserPlansAsync()).ThrowsAsync(new Exception());
            var controller = new UserPlanController(_serviceMock.Object);
            var result = await controller.GetAllUserPlansAsync();
            Assert.That(result.Result, Is.InstanceOf<ObjectResult>());
        }

        [Test]
        public async Task GetUserPlanByIdAsync_Valid_ReturnsOk()
        {
            _serviceMock.Setup(s => s.GetUserPlanByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new UserPlanResponseDto());
            var result = await _controller.GetUserPlanByIdAsync(Guid.NewGuid());
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task GetUserPlanByIdAsync_InvalidId_ReturnsBadRequest()
        {
            var result = await _controller.GetUserPlanByIdAsync(Guid.Empty);
            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task GetUserPlanByIdAsync_Exception_ReturnsStatus500()
        {
            _serviceMock.Setup(s => s.GetUserPlanByIdAsync(It.IsAny<Guid>())).ThrowsAsync(new Exception());
            var result = await _controller.GetUserPlanByIdAsync(Guid.NewGuid());
            Assert.That(result.Result, Is.InstanceOf<ObjectResult>());
        }

        [Test]
        public async Task AddUserPlan_Valid_ReturnsOk()
        {
            var dto = new UserPlanAddRequestDto { UserId = Guid.NewGuid(), WorkOutPlanId = Guid.NewGuid() };
            var result = await _controller.AddUserPlan(dto);
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task AddUserPlan_InvalidModel_ReturnsBadRequest()
        {
            _controller.ModelState.AddModelError("error", "Invalid");
            var result = await _controller.AddUserPlan(new UserPlanAddRequestDto());
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task AddUserPlan_Exception_ReturnsStatus500()
        {
            _serviceMock.Setup(s => s.AddUserPlanAsync(It.IsAny<UserPlanAddRequestDto>())).ThrowsAsync(new Exception());
            var result = await _controller.AddUserPlan(new UserPlanAddRequestDto { UserId = Guid.NewGuid(), WorkOutPlanId = Guid.NewGuid() });
            Assert.That(result, Is.InstanceOf<ObjectResult>());
        }

        [Test]
        public async Task DeleteUserPlan_Valid_ReturnsOk()
        {
            var result = await _controller.DeleteUserPlan(Guid.NewGuid());
            Assert.That(result,Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task DeleteUserPlan_Invalid_ReturnsBadRequest()
        {
            var result = await _controller.DeleteUserPlan(Guid.Empty);
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task DeleteUserPlan_Exception_ReturnsStatus500()
        {
            _serviceMock.Setup(s => s.DeleteUserPlanAsync(It.IsAny<Guid>())).ThrowsAsync(new Exception());
            var result = await _controller.DeleteUserPlan(Guid.NewGuid());
            Assert.That(result, Is.InstanceOf<ObjectResult>());
        }

        [Test]
        public async Task UpdateUserPlan_Valid_ReturnsOk()
        {
            var result = await _controller.UpdateUserPlan(Guid.NewGuid(), new UserPlanUpdateDto());
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task UpdateUserPlan_Invalid_ReturnsBadRequest()
        {
            var result = await _controller.UpdateUserPlan(Guid.Empty, null);
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task UpdateUserPlan_Exception_ReturnsStatus500()
        {
            _serviceMock.Setup(s => s.UpdateUserPlanAsync(It.IsAny<Guid>(), It.IsAny<UserPlanUpdateDto>())).ThrowsAsync(new Exception());
            var result = await _controller.UpdateUserPlan(Guid.NewGuid(), new UserPlanUpdateDto());
            Assert.That(result, Is.InstanceOf<ObjectResult>());
        }

        [Test]
        public async Task GetUserPlansByUserIdAsync_Valid_ReturnsOk()
        {
            _serviceMock.Setup(s => s.GetUserPlansByUserIdAsync(It.IsAny<Guid>())).ReturnsAsync(new List<UserPlanResponseDto> { new() });
            var result = await _controller.GetUserPlansByUserIdAsync(Guid.NewGuid());
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task GetUserPlansByUserIdAsync_InvalidId_ReturnsBadRequest()
        {
            var result = await _controller.GetUserPlansByUserIdAsync(Guid.Empty);
            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task GetUserPlansByUserIdAsync_Exception_ReturnsStatus500()
        {
            _serviceMock.Setup(s => s.GetUserPlansByUserIdAsync(It.IsAny<Guid>())).ThrowsAsync(new Exception());
            var result = await _controller.GetUserPlansByUserIdAsync(Guid.NewGuid());
            Assert.That(result.Result, Is.InstanceOf<ObjectResult>());
        }

        [Test]
        public async Task GetPaginatedUserPlansAsync_Valid_ReturnsOk()
        {
            _serviceMock.Setup(s => s.GetPaginatedUserPlansAsync(It.IsAny<UserWorkOutPlanFilterDto>())).ReturnsAsync(new PaginatedResult<UserPlanResponseDto> { Data = new List<UserPlanResponseDto> { new() } });
            var result = await _controller.GetPaginatedUserPlansAsync(new UserWorkOutPlanFilterDto());
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task GetPaginatedUserPlansAsync_NullFilter_ReturnsBadRequest()
        {
            var result = await _controller.GetPaginatedUserPlansAsync(null);
            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task GetPaginatedUserPlansAsync_Exception_ReturnsStatus500()
        {
            _serviceMock.Setup(s => s.GetPaginatedUserPlansAsync(It.IsAny<UserWorkOutPlanFilterDto>())).ThrowsAsync(new Exception());
            var result = await _controller.GetPaginatedUserPlansAsync(new UserWorkOutPlanFilterDto());
            Assert.That(result.Result, Is.InstanceOf<ObjectResult>());
        }
    }
}
