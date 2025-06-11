
using FitnessTracking.Controllers;
using FitnessTracking.Interfaces;
using FitnessTracking.Models;
using FitnessTracking.Models.DTOs;
using FitnessTracking.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using NUnit.Framework;

namespace FitnessTracking.Controller
{
    [TestFixture]
    public class AuthenticationControllerTests
    {
        private Mock<IAuthenticationService> _mockAuthService;
        private Mock<ITokenService> _mockTokenService;
        private Mock<UserRepository> _mockUserRepository;
        private AuthenticationController _controller;

        [SetUp]
        public void Setup()
        {
            _mockAuthService = new Mock<IAuthenticationService>();
            _mockTokenService = new Mock<ITokenService>();
            _mockUserRepository = new Mock<UserRepository>(null);
            _controller = new AuthenticationController(_mockAuthService.Object, _mockTokenService.Object, _mockUserRepository.Object);
        }

        [Test]
        public async Task Login_ValidCredentials_ReturnsOk()
        {
            var request = new UserLoginRequestDto { Email = "test@example.com", Password = "pass" };
            _mockAuthService.Setup(s => s.Login(request)).ReturnsAsync(new UserLoginResponseDto());

            var result = await _controller.Login(request);

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task Login_InvalidCredentials_ReturnsUnauthorized()
        {
            var request = new UserLoginRequestDto { Email = "fail@example.com", Password = "wrong" };
            _mockAuthService.Setup(s => s.Login(request)).ThrowsAsync(new Exception("Invalid credentials"));

            var result = await _controller.Login(request);

            Assert.That(result, Is.InstanceOf<UnauthorizedObjectResult>());
        }

        [Test]
        public async Task Login_ExceptionThrown_ReturnsUnauthorized()
        {
            var request = new UserLoginRequestDto();
            _mockAuthService.Setup(s => s.Login(It.IsAny<UserLoginRequestDto>())).ThrowsAsync(new Exception("Unexpected error"));

            var result = await _controller.Login(request);

            Assert.That(result, Is.InstanceOf<UnauthorizedObjectResult>());
        }

        [Test]
        public async Task Refresh_ValidToken_ReturnsOk()
        {
            var email = "test@example.com";
            var user = new UserModel { Email = email };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("supersecretkey123supersecretkey123")); // 32 bytes
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, email),
                new Claim(JwtRegisteredClaimNames.Exp, DateTimeOffset.UtcNow.AddMinutes(10).ToUnixTimeSeconds().ToString())
                 };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(10),
                signingCredentials: creds
            );

            var handler = new JwtSecurityTokenHandler();
            var tokenString = handler.WriteToken(token);

            _mockTokenService.Setup(t => t.GetSecurityKey()).Returns(key);
            _mockTokenService.Setup(t => t.GenerateAccessToken(It.IsAny<UserModel>())).Returns("new.access.token");
            _mockUserRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<UserModel> { user });

            var refreshDto = new RefreshTokenRequestDto { RefreshToken = tokenString };
            var result = await _controller.Refresh(refreshDto);

            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.Value, Is.InstanceOf<RefreshTokenResponseDto>());
        }


        [Test]
        public async Task Refresh_MissingToken_ReturnsBadRequest()
        {
            var refreshDto = new RefreshTokenRequestDto { RefreshToken = "" };

            var result = await _controller.Refresh(refreshDto);

            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task Refresh_ExceptionThrown_ReturnsUnauthorized()
        {
            var refreshDto = new RefreshTokenRequestDto { RefreshToken = "bad.token" };
            _mockTokenService.Setup(t => t.GetSecurityKey()).Throws(new Exception("Token error"));

            var result = await _controller.Refresh(refreshDto);

            Assert.That(result, Is.InstanceOf<UnauthorizedObjectResult>());
        }
    }
}
