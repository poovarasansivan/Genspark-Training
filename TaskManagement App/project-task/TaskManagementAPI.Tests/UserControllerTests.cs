using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagementAPI.ApiResponses;
using TaskManagementAPI.Controllers;
using TaskManagementAPI.CustomExceptions;
using TaskManagementAPI.DTOs.Users;
using TaskManagementAPI.Interfaces;

namespace TaskManagementAPI.Tests.Controllers
{
    [TestFixture]
    public class UserControllerTests
    {
        private Mock<IUserService> _userServiceMock;
        private UserController _controller;

        [SetUp]
        public void Setup()
        {
            _userServiceMock = new Mock<IUserService>();
            _controller = new UserController(_userServiceMock.Object);
        }

        private void SetUserContext(Guid userId)
        {
            var httpContext = new DefaultHttpContext();
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            };
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims));
            _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
        }

        [Test]
        public async Task GetAllUsers_ReturnsPagedUsers()
        {
            // Arrange
            var users = new List<UserResponseDto> { new UserResponseDto { Id = Guid.NewGuid(), Email = "test@example.com" } };
            var pagedResponse = PagedResponse<UserResponseDto>.Create(users, 1, 10, 1);
            pagedResponse.Message = "Users retrieved";

            _userServiceMock.Setup(s => s.GetAllAsync(1, 10)).ReturnsAsync(pagedResponse);

            // Act
            var result = await _controller.GetAllUsers(1, 10);
            var okResult = result.Result as OkObjectResult;

            // Assert
            Assert.IsNotNull(okResult);
            var response = okResult.Value as PagedResponse<UserResponseDto>;
            Assert.IsNotNull(response);
            Assert.AreEqual(1, response.Pagination.Page);
            Assert.AreEqual(10, response.Pagination.PageSize);
            Assert.AreEqual("Users fetched successfully.", response.Message);
        }

        [Test]
        public async Task GetUserById_UserExists_ReturnsUser()
        {
            var userId = Guid.NewGuid();
            var user = new UserResponseDto { Id = userId, Email = "test@example.com" };

            _userServiceMock.Setup(s => s.GetByIdAsync(userId)).ReturnsAsync(ApiResponse<UserResponseDto>.SuccessResponse(user));

            var result = await _controller.GetUserById(userId);
            var okResult = result.Result as OkObjectResult;

            Assert.IsNotNull(okResult);
            var response = okResult.Value as ApiResponse<UserResponseDto>;
            Assert.IsTrue(response.Success);
            Assert.AreEqual(userId, response.Data.Id);
        }

        [Test]
        public async Task GetUserById_UserNotFound_ReturnsNotFound()
        {
            var userId = Guid.NewGuid();
            var errorResponse = ApiResponse<UserResponseDto>.ErrorResponse("Not found", new Dictionary<string, List<string>>());

            _userServiceMock.Setup(s => s.GetByIdAsync(userId)).ReturnsAsync(errorResponse);

            var result = await _controller.GetUserById(userId);
            Assert.IsInstanceOf<NotFoundObjectResult>(result.Result);
        }

        [Test]
        public async Task CreateUser_ValidInput_ReturnsCreated()
        {
            var userId = Guid.NewGuid();
            SetUserContext(userId);

            var request = new UserRequestDto { FullName = "Test User", Email = "test@example.com", Password = "pass123" };
            var responseUser = new UserResponseDto { Id = Guid.NewGuid(), FullName = request.FullName, Email = request.Email };

            _userServiceMock.Setup(s => s.CreateAsync(request, userId))
                .ReturnsAsync(ApiResponse<UserResponseDto>.SuccessResponse(responseUser));

            var result = await _controller.CreateUser(request);
            var createdResult = result.Result as CreatedAtActionResult;

            Assert.IsNotNull(createdResult);
            var response = createdResult.Value as ApiResponse<UserResponseDto>;
            Assert.AreEqual(request.Email, response.Data.Email);
        }

        [Test]
        public async Task CreateUser_ConflictException_ReturnsConflict()
        {
            var userId = Guid.NewGuid();
            SetUserContext(userId);
            var dto = new UserRequestDto { Email = "conflict@example.com" };

            _userServiceMock.Setup(s => s.CreateAsync(dto, userId))
                .ThrowsAsync(new ConflictException("Email already exists"));

            var result = await _controller.CreateUser(dto);
            var conflictResult = result.Result as ConflictObjectResult;

            Assert.IsNotNull(conflictResult);
            var response = conflictResult.Value as ApiResponse<UserResponseDto>;
            Assert.AreEqual("Email already exists", response.Message);
        }

        [Test]
        public async Task UpdateUser_ValidInput_ReturnsOk()
        {
            var userId = Guid.NewGuid();
            SetUserContext(userId);

            var updateDto = new UserUpdateDto { FullName = "Updated Name" };
            var updatedUser = new UserResponseDto { Id = userId, FullName = "Updated Name" };

            _userServiceMock.Setup(s => s.UpdateAsync(userId, updateDto, userId))
                .ReturnsAsync(ApiResponse<UserResponseDto>.SuccessResponse(updatedUser));

            var result = await _controller.UpdateUser(userId, updateDto);
            var okResult = result.Result as OkObjectResult;

            Assert.IsNotNull(okResult);
            var response = okResult.Value as ApiResponse<UserResponseDto>;
            Assert.AreEqual("Updated Name", response.Data.FullName);
        }

        [Test]
        public async Task DeleteUser_ValidId_ReturnsSuccess()
        {
            var userId = Guid.NewGuid();
            var response = ApiResponse<string>.SuccessResponse("Deleted");

            _userServiceMock.Setup(s => s.DeleteAsync(userId)).ReturnsAsync(response);

            var result = await _controller.DeleteUser(userId);
            var okResult = result.Result as OkObjectResult;

            Assert.IsNotNull(okResult);
            var responseData = okResult.Value as ApiResponse<string>;
            Assert.AreEqual("Deleted", responseData.Data);
        }
    }
}
