using NUnit.Framework;
using Moq;
using Microsoft.AspNetCore.Mvc;
using FitnessTracking.Controllers;
using FitnessTracking.Interfaces;
using FitnessTracking.Models.DTOs;
using FitnessTracking.Models;
using FitnessTracking.Misc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace FitnessTracking.Controller
{
    [TestFixture]
    public class WorkOutPlanControllerTests
    {
        private Mock<IWorkOutPlanService> _mockService;
        private WorkOutPlanController _controller;

        [SetUp]
        public void Setup()
        {
            _mockService = new Mock<IWorkOutPlanService>();
            _controller = new WorkOutPlanController(_mockService.Object);
        }


        [Test]
        public async Task GetAllWorkOutPlansAsync_Valid_ReturnsOk()
        {
            _mockService.Setup(s => s.GetAllWorkOutPlansAsync()).ReturnsAsync(new List<WorkOutResponeDto> { new() });
            var result = await _controller.GetAllWorkOutPlansAsync();
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task GetAllWorkOutPlansAsync_Invalid_ReturnsEmpty()
        {
            _mockService.Setup(s => s.GetAllWorkOutPlansAsync()).ReturnsAsync(new List<WorkOutResponeDto>());
            var result = await _controller.GetAllWorkOutPlansAsync();
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public void GetAllWorkOutPlansAsync_Exception_Throws()
        {
            _mockService.Setup(s => s.GetAllWorkOutPlansAsync()).ThrowsAsync(new Exception());
            Assert.ThrowsAsync<Exception>(() => _controller.GetAllWorkOutPlansAsync());
        }

        [Test]
        public async Task GetWorkOutPlanByIdAsync_Valid_ReturnsOk()
        {
            _mockService.Setup(s => s.GetWorkOutPlanByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new WorkOutResponeDto());
            var result = await _controller.GetWorkOutPlanByIdAsync(Guid.NewGuid());
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        }


        [Test]
        public void GetWorkOutPlanByIdAsync_Exception_Throws()
        {
            _mockService.Setup(s => s.GetWorkOutPlanByIdAsync(It.IsAny<Guid>())).ThrowsAsync(new Exception());
            Assert.ThrowsAsync<Exception>(() => _controller.GetWorkOutPlanByIdAsync(Guid.NewGuid()));
        }


        [Test]
        public async Task AddWorkOutPlan_Valid_ReturnsOk()
        {
            var dto = new WorkOutAddRequestDto();
            var result = await _controller.AddWorkOutPlan(dto);
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task AddWorkOutPlan_InvalidModelState_ReturnsBadRequest()
        {
            _controller.ModelState.AddModelError("Name", "Required");
            var result = await _controller.AddWorkOutPlan(new WorkOutAddRequestDto());
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public void AddWorkOutPlan_Exception_Throws()
        {
            _mockService.Setup(s => s.AddWorkOutPlanAsync(It.IsAny<WorkOutAddRequestDto>())).ThrowsAsync(new Exception());
            Assert.ThrowsAsync<Exception>(() => _controller.AddWorkOutPlan(new WorkOutAddRequestDto()));
        }

        [Test]
        public async Task DeleteWorkOutPlan_Valid_ReturnsOk()
        {
            var result = await _controller.DeleteWorkOutPlan(Guid.NewGuid());
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public void DeleteWorkOutPlan_Invalid_ThrowsCustomeException()
        {
            var ex = Assert.ThrowsAsync<CustomeExceptionHandler>(() => _controller.DeleteWorkOutPlan(Guid.Empty));
            Assert.That(ex!.Message, Is.EqualTo("Workout Plan ID could not be empty or null"));
        }

        [Test]
        public void DeleteWorkOutPlan_Exception_Throws()
        {
            _mockService.Setup(s => s.DeleteWorkOutPlanAsync(It.IsAny<Guid>())).ThrowsAsync(new Exception());
            Assert.ThrowsAsync<Exception>(() => _controller.DeleteWorkOutPlan(Guid.NewGuid()));
        }



        [Test]
        public async Task UpdateWorkOutPlan_Valid_ReturnsOk()
        {
            var result = await _controller.UpdateWorkOutPlan(Guid.NewGuid(), new WorkOutPlanUpdateDto());
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public void UpdateWorkOutPlan_InvalidId_ThrowsCustomeException()
        {
            var ex = Assert.ThrowsAsync<CustomeExceptionHandler>(() => _controller.UpdateWorkOutPlan(Guid.Empty, new WorkOutPlanUpdateDto()));
            Assert.That(ex!.Message, Is.EqualTo("Workout Plan ID could not be empty or null"));
        }

        [Test]
        public async Task UpdateWorkOutPlan_InvalidModelState_ReturnsBadRequest()
        {
            _controller.ModelState.AddModelError("Description", "Required");
            var result = await _controller.UpdateWorkOutPlan(Guid.NewGuid(), new WorkOutPlanUpdateDto());
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }


        [Test]
        public async Task GetWorkOutPlansByFilterAsync_Valid_ReturnsOk()
        {
            var dto = new PaginatedResult<WorkOutResponeDto> { Data = new List<WorkOutResponeDto> { new() } };
            _mockService.Setup(s => s.GetFilteredWorkOutPlansAsync(It.IsAny<WorkOutPlanFilterDto>())).ReturnsAsync(dto);
            var result = await _controller.GetWorkOutPlansByFilterAsync(new WorkOutPlanFilterDto());
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task GetWorkOutPlansByFilterAsync_Invalid_ReturnsNotFound()
        {
            var dto = new PaginatedResult<WorkOutResponeDto> { Data = new List<WorkOutResponeDto>() };
            _mockService.Setup(s => s.GetFilteredWorkOutPlansAsync(It.IsAny<WorkOutPlanFilterDto>())).ReturnsAsync(dto);
            var result = await _controller.GetWorkOutPlansByFilterAsync(new WorkOutPlanFilterDto());
            Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public void GetWorkOutPlansByFilterAsync_Exception_Throws()
        {
            _mockService.Setup(s => s.GetFilteredWorkOutPlansAsync(It.IsAny<WorkOutPlanFilterDto>())).ThrowsAsync(new Exception());
            Assert.ThrowsAsync<Exception>(() => _controller.GetWorkOutPlansByFilterAsync(new WorkOutPlanFilterDto()));
        }
    }
}
