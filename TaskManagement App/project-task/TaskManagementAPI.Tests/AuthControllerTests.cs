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
using TaskManagementAPI.DTOs.Auth;
using TaskManagementAPI.Enums;
using TaskManagementAPI.Interfaces.Auth;

namespace TaskManagementAPI.Tests.Controllers
{
    [TestFixture]
    public class AuthControllerTests
    {
        private Mock<IAuthenticationService> _authServiceMock;
        private AuthController _controller;

        [SetUp]
        public void Setup()
        {
            _authServiceMock = new Mock<IAuthenticationService>();
            _controller = new AuthController(_authServiceMock.Object);
        }

        [Test]
        public async Task Login_ValidCredentials_ReturnsSuccess()
        {
            // Arrange
            var loginDto = new LoginRequestDto { Email = "test@example.com", Password = "password" };
            var loginResponse = new LoginResponseDto { AccessToken = "access-token", RefreshToken = "refresh-token" };

            _authServiceMock.Setup(s => s.LoginAsync(loginDto)).ReturnsAsync(loginResponse);

            // Act
            var actionResult = await _controller.Login(loginDto);
            var result = actionResult.Result as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            var apiResponse = result.Value as ApiResponse<LoginResponseDto>;
            Assert.IsNotNull(apiResponse);
            Assert.AreEqual("access-token", apiResponse.Data.AccessToken);
        }

        [Test]
        public async Task Login_InvalidCredentials_ReturnsBadRequest()
        {
            // Arrange
            var loginDto = new LoginRequestDto { Email = "wrong@example.com", Password = "wrongpass" };
            _authServiceMock.Setup(s => s.LoginAsync(loginDto))
                .ThrowsAsync(new UnauthorizedException("Invalid credentials"));

            // Act
            var actionResult = await _controller.Login(loginDto);
            var result = actionResult.Result as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(result);
            var apiResponse = result.Value as ApiResponse<string>;
            Assert.IsNotNull(apiResponse);
            Assert.AreEqual("Invalid credentials", apiResponse.Message);
        }

        [Test]
        public async Task RefreshToken_ValidRefreshToken_ReturnsNewTokens()
        {
            // Arrange
            var refreshDto = new RefreshTokenRequestDto { RefreshToken = "valid-refresh-token" };
            var loginResponse = new LoginResponseDto { AccessToken = "new-token", RefreshToken = "new-refresh-token" };

            _authServiceMock.Setup(s => s.RefreshTokenAsync(refreshDto)).ReturnsAsync(loginResponse);

            // Act
            var actionResult = await _controller.RefreshToken(refreshDto);
            var result = actionResult.Result as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            var apiResponse = result.Value as ApiResponse<LoginResponseDto>;
            Assert.IsNotNull(apiResponse);
            Assert.AreEqual("new-token", apiResponse.Data.AccessToken);
        }

        [Test]
        public async Task RefreshToken_InvalidRefreshToken_ReturnsUnauthorized()
        {
            // Arrange
            var refreshDto = new RefreshTokenRequestDto { RefreshToken = "invalid" };
            _authServiceMock.Setup(s => s.RefreshTokenAsync(refreshDto))
                .ThrowsAsync(new UnauthorizedException("Invalid token"));

            // Act
            var actionResult = await _controller.RefreshToken(refreshDto);
            var result = actionResult.Result as UnauthorizedObjectResult;

            // Assert
            Assert.IsNotNull(result);
            var apiResponse = result.Value as ApiResponse<string>;
            Assert.AreEqual("Invalid token", apiResponse.Message);
        }

        [Test]
        public async Task Logout_ValidToken_ReturnsSuccess()
        {
            // Arrange
            var token = "refresh-token";
            _authServiceMock.Setup(s => s.LogoutAsync(token)).Returns(Task.CompletedTask);

            // Act
            var actionResult = await _controller.Logout(token);
            var result = actionResult.Result as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            var apiResponse = result.Value as ApiResponse<string>;
            Assert.AreEqual("Logout successful", apiResponse.Message);
        }

        [Test]
        public async Task GetCurrentUser_ValidToken_ReturnsUserDetails()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var expectedUser = new MeResponseDto
            {
                Id = userId,
                Email = "test@example.com",
                FullName = "Test User",
                Role = UserRole.TeamMember.ToString()
            };

            _authServiceMock.Setup(s => s.GetMeAsync(userId.ToString())).ReturnsAsync(expectedUser);

            // Mock user identity
            var httpContext = new DefaultHttpContext();
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, UserRole.TeamMember.ToString())
            };
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims));
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var actionResult = await _controller.GetCurrentUser();
            var result = actionResult.Result as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            var apiResponse = result.Value as ApiResponse<MeResponseDto>;
            Assert.IsNotNull(apiResponse);
            Assert.AreEqual(expectedUser.Id, apiResponse.Data.Id);
        }
    }
}
