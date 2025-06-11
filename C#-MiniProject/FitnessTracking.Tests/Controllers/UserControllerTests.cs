using FitnessTracking.Controllers;
using FitnessTracking.Interfaces;
using FitnessTracking.Models;
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
    public class UserControllerTests
    {
        private Mock<IUserService> _mockUserService;
        private UserController _controller;

        [SetUp]
        public void SetUp()
        {
            _mockUserService = new Mock<IUserService>();
            _controller = new UserController(_mockUserService.Object);
        }

        [Test]
        public async Task GetByUserId_ValidId_ReturnsUser()
        {
            var userId = Guid.NewGuid();
            var userResponseDto = new UserResponseDto
            {
                Id = userId,
                Name = "Test User",
                Email = "test@example.com"
            };

            _mockUserService.Setup(s => s.GetUserByIdAsync(userId)).ReturnsAsync(userResponseDto);

            var result = await _controller.GetByUserId(userId);

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        }


        [Test]
        public async Task GetByUserId_InvalidId_ReturnsNotFound()
        {
            _mockUserService.Setup(s => s.GetUserByIdAsync(It.IsAny<Guid>())).ReturnsAsync((UserResponseDto)null);

            var result = await _controller.GetByUserId(Guid.NewGuid());

            Assert.That(result.Result, Is.InstanceOf<NotFoundResult>());
        }


        [Test]
        public void GetByUserId_ExceptionThrown_ReturnsException()
        {
            var userId = Guid.NewGuid();
            _mockUserService.Setup(s => s.GetUserByIdAsync(userId)).ThrowsAsync(new Exception());

            Assert.ThrowsAsync<Exception>(() => _controller.GetByUserId(userId));
        }

        [Test]
        public async Task GetAllUsersAsync_Valid_ReturnsUsers()
        {
            _mockUserService.Setup(s => s.GetAllUsersAsync()).ReturnsAsync(new List<UserResponseDto> { new UserResponseDto { Id = Guid.NewGuid(), Name = "Test" } });

            var result = await _controller.GetAllUsersAsync();

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        }

        [Test]
        public async Task GetAllUsersAsync_NoUsers_ReturnsNotFound()
        {
            _mockUserService.Setup(s => s.GetAllUsersAsync()).ReturnsAsync((IEnumerable<UserResponseDto>)null);

            var result = await _controller.GetAllUsersAsync();

            Assert.That(result.Result, Is.InstanceOf<NotFoundResult>());
        }


        [Test]
        public void GetAllUsersAsync_ExceptionThrown_ReturnsException()
        {
            _mockUserService.Setup(s => s.GetAllUsersAsync()).ThrowsAsync(new Exception());

            Assert.ThrowsAsync<Exception>(() => _controller.GetAllUsersAsync());
        }

        [Test]
        public async Task AddUser_Valid_ReturnsOk()
        {
            var userDto = new UserRegisterDto();
            _controller.ModelState.Clear();

            var result = await _controller.AddUser(userDto);

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task AddUser_InvalidModel_ReturnsBadRequest()
        {
            _controller.ModelState.AddModelError("Email", "Required");

            var result = await _controller.AddUser(new UserRegisterDto());

            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public void AddUser_ExceptionThrown_ReturnsException()
        {
            _mockUserService.Setup(s => s.AddUserAsync(It.IsAny<UserRegisterDto>())).ThrowsAsync(new Exception());

            Assert.ThrowsAsync<Exception>(() => _controller.AddUser(new UserRegisterDto()));
        }

        [Test]
        public async Task UpdateUser_Valid_ReturnsOk()
        {
            var id = Guid.NewGuid();

            var result = await _controller.UpdateUser(id, new UpdateUserDto());

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public void UpdateUser_NullId_ThrowsCustomException()
        {
            Assert.ThrowsAsync<CustomeExceptionHandler>(() => _controller.UpdateUser(Guid.Empty, new UpdateUserDto()));
        }

        [Test]
        public void UpdateUser_ExceptionThrown_ReturnsException()
        {
            _mockUserService.Setup(s => s.UpdateUserAsync(It.IsAny<Guid>(), It.IsAny<UpdateUserDto>())).ThrowsAsync(new Exception());

            Assert.ThrowsAsync<Exception>(() => _controller.UpdateUser(Guid.NewGuid(), new UpdateUserDto()));
        }

        [Test]
        public async Task UpdatePassword_Valid_ReturnsOk()
        {
            var result = await _controller.UpdatePassword(Guid.NewGuid(), new UpdatePasswordDto());

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public void UpdatePassword_NullId_ThrowsCustomException()
        {
            Assert.ThrowsAsync<CustomeExceptionHandler>(() => _controller.UpdatePassword(Guid.Empty, new UpdatePasswordDto()));
        }

        [Test]
        public void UpdatePassword_ExceptionThrown_ReturnsException()
        {
            _mockUserService.Setup(s => s.UpdatePasswordAsync(It.IsAny<Guid>(), It.IsAny<UpdatePasswordDto>())).ThrowsAsync(new Exception());

            Assert.ThrowsAsync<Exception>(() => _controller.UpdatePassword(Guid.NewGuid(), new UpdatePasswordDto()));
        }

        [Test]
        public async Task DeleteUser_Valid_ReturnsOk()
        {
            var result = await _controller.DeleteUser(Guid.NewGuid());

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public void DeleteUser_NullId_ThrowsCustomException()
        {
            Assert.ThrowsAsync<CustomeExceptionHandler>(() => _controller.DeleteUser(Guid.Empty));
        }

        [Test]
        public void DeleteUser_ExceptionThrown_ReturnsException()
        {
            _mockUserService.Setup(s => s.DeleteUserAsync(It.IsAny<Guid>())).ThrowsAsync(new Exception());

            Assert.ThrowsAsync<Exception>(() => _controller.DeleteUser(Guid.NewGuid()));
        }

        [Test]
        public async Task UpdateUserStatus_Valid_ReturnsOk()
        {
            var result = await _controller.UpdateUserStatus(Guid.NewGuid(), new UpdateUserStatusDto());

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public void UpdateUserStatus_NullId_ThrowsCustomException()
        {
            Assert.ThrowsAsync<CustomeExceptionHandler>(() => _controller.UpdateUserStatus(Guid.Empty, new UpdateUserStatusDto()));
        }

        [Test]
        public void UpdateUserStatus_ExceptionThrown_ReturnsException()
        {
            _mockUserService.Setup(s => s.UpdateUserStatusAsyn(It.IsAny<Guid>(), It.IsAny<UpdateUserStatusDto>())).ThrowsAsync(new Exception());

            Assert.ThrowsAsync<Exception>(() => _controller.UpdateUserStatus(Guid.NewGuid(), new UpdateUserStatusDto()));
        }

        [Test]
        public async Task GetPaginatedAllUsersAsync_Valid_ReturnsOk()
        {
            _mockUserService.Setup(s => s.GetPaginatedAllUsersAsync(It.IsAny<PaginationParameterDto>())).ReturnsAsync(new PaginatedResult<UserResponseDto>());

            var result = await _controller.GetPaginatedAllUsersAsync(new PaginationParameterDto());

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public void GetPaginatedAllUsersAsync_NullParams_ThrowsCustomException()
        {
            Assert.ThrowsAsync<CustomeExceptionHandler>(() => _controller.GetPaginatedAllUsersAsync(null));
        }

        [Test]
        public void GetPaginatedAllUsersAsync_ExceptionThrown_ReturnsException()
        {
            _mockUserService.Setup(s => s.GetPaginatedAllUsersAsync(It.IsAny<PaginationParameterDto>())).ThrowsAsync(new Exception());

            Assert.ThrowsAsync<Exception>(() => _controller.GetPaginatedAllUsersAsync(new PaginationParameterDto()));
        }
    }
}
