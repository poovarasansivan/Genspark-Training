using NUnit.Framework;
using Moq;
using System;
using System.Threading.Tasks;
using TaskManagementAPI.DTOs.Auth;
using TaskManagementAPI.Interfaces;
using TaskManagementAPI.Interfaces.Auth;
using TaskManagementAPI.Services;
using TaskManagementAPI.Mapper;
using TaskManagementAPI.Models;
using TaskManagementAPI.CustomExceptions;

namespace TaskManagementAPI.Tests.Services
{
    [TestFixture]
    public class AuthenticationServiceTests
    {
        private Mock<IUserRepository> _userRepoMock;
        private Mock<ITokenService> _tokenServiceMock;
        private Mock<IRefreshTokenRepository> _refreshTokenRepoMock;
        private AuthMapper _authMapper;
        private AuthenticationService _authService;

        [SetUp]
        public void Setup()
        {
            _userRepoMock = new Mock<IUserRepository>();
            _tokenServiceMock = new Mock<ITokenService>();
            _refreshTokenRepoMock = new Mock<IRefreshTokenRepository>();
            _authMapper = new AuthMapper();

            _authService = new AuthenticationService(
                _userRepoMock.Object,
                _tokenServiceMock.Object,
                _authMapper,
                _refreshTokenRepoMock.Object
            );
        }

        [Test]
        public async Task LoginAsync_WithValidCredentials_ReturnsLoginResponse()
        {
            var loginDto = new LoginRequestDto
            {
                Email = "test@example.com",
                Password = "password123"
            };

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "test@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                FullName = "Test User",
                Role = TaskManagementAPI.Enums.UserRole.TeamMember
            };

            _userRepoMock.Setup(x => x.GetByEmail(loginDto.Email))
                         .ReturnsAsync(user);

            _tokenServiceMock.Setup(x => x.GenerateAccessToken(It.IsAny<User>()))
                             .ReturnsAsync("access-token");
            _tokenServiceMock.Setup(x => x.GenerateRefreshToken())
                             .ReturnsAsync("refresh-token");

            _refreshTokenRepoMock.Setup(x => x.Add(It.IsAny<RefreshToken>()))
                                 .Returns(Task.CompletedTask);

            var result = await _authService.LoginAsync(loginDto);

            Assert.AreEqual(user.Email, result.User.Email);
            Assert.AreEqual("access-token", result.AccessToken);
            Assert.AreEqual("refresh-token", result.RefreshToken);
        }

        [Test]
        public void LoginAsync_WithInvalidPassword_ThrowsUnauthorizedException()
        {
            var loginDto = new LoginRequestDto
            {
                Email = "test@example.com",
                Password = "wrongpassword"
            };

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "test@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("correctpassword"),
                FullName = "Test User",
                Role = TaskManagementAPI.Enums.UserRole.TeamMember
            };

            _userRepoMock.Setup(x => x.GetByEmail(loginDto.Email))
                         .ReturnsAsync(user);

            Assert.ThrowsAsync<UnauthorizedException>(() => _authService.LoginAsync(loginDto));
        }

        [Test]
        public void LoginAsync_WithNonExistingUser_ThrowsUnauthorizedException()
        {
            var loginDto = new LoginRequestDto
            {
                Email = "nonexistent@example.com",
                Password = "password"
            };

            _userRepoMock.Setup(x => x.GetByEmail(loginDto.Email))
                         .ReturnsAsync((User)null!);

            Assert.ThrowsAsync<UnauthorizedException>(() => _authService.LoginAsync(loginDto));
        }

        [Test]
        public async Task RefreshTokenAsync_WithValidToken_ReturnsNewTokens()
        {
            var userId = Guid.NewGuid();
            var oldToken = new RefreshToken
            {
                Token = "old-token",
                UserId = userId,
                IsRevoked = false,
                ExpiresAt = DateTime.UtcNow.AddDays(1)
            };

            var user = new User
            {
                Id = userId,
                FullName = "Test User",
                Email = "test@example.com",
                PasswordHash = "hash",
                Role = TaskManagementAPI.Enums.UserRole.TeamMember
            };

            _refreshTokenRepoMock.Setup(x => x.GetByToken("old-token"))
                                 .ReturnsAsync(oldToken);
            _userRepoMock.Setup(x => x.Get(userId))
                         .ReturnsAsync(user);
            _refreshTokenRepoMock.Setup(x => x.Update(It.IsAny<RefreshToken>()))
                                 .Returns(Task.CompletedTask);
            _refreshTokenRepoMock.Setup(x => x.Add(It.IsAny<RefreshToken>()))
                                 .Returns(Task.CompletedTask);
            _tokenServiceMock.Setup(x => x.GenerateAccessToken(user))
                             .ReturnsAsync("new-access-token");
            _tokenServiceMock.Setup(x => x.GenerateRefreshToken())
                             .ReturnsAsync("new-refresh-token");

            var request = new RefreshTokenRequestDto { RefreshToken = "old-token" };

            var result = await _authService.RefreshTokenAsync(request);

            Assert.AreEqual("new-access-token", result.AccessToken);
            Assert.AreEqual("new-refresh-token", result.RefreshToken);
        }

        [Test]
        public void RefreshTokenAsync_WithRevokedToken_ThrowsUnauthorizedException()
        {
            var token = new RefreshToken
            {
                Token = "expired",
                IsRevoked = true,
                ExpiresAt = DateTime.UtcNow.AddDays(-1)
            };

            _refreshTokenRepoMock.Setup(x => x.GetByToken("expired"))
                                 .ReturnsAsync(token);

            var request = new RefreshTokenRequestDto { RefreshToken = "expired" };

            Assert.ThrowsAsync<UnauthorizedException>(() => _authService.RefreshTokenAsync(request));
        }

        [Test]
        public async Task LogoutAsync_WithValidToken_RevokesToken()
        {
            var token = new RefreshToken
            {
                Token = "logout-token",
                IsRevoked = false
            };

            _refreshTokenRepoMock.Setup(x => x.GetByToken("logout-token"))
                                 .ReturnsAsync(token);
            _refreshTokenRepoMock.Setup(x => x.Update(token))
                                 .Returns(Task.CompletedTask);

            await _authService.LogoutAsync("logout-token");

            Assert.IsTrue(token.IsRevoked);
        }

        [Test]
        public void LogoutAsync_WithInvalidToken_ThrowsUnauthorizedException()
        {
            _refreshTokenRepoMock.Setup(x => x.GetByToken("invalid-token"))
                                 .ReturnsAsync((RefreshToken)null!);

            Assert.ThrowsAsync<UnauthorizedException>(() => _authService.LogoutAsync("invalid-token"));
        }

        [Test]
        public async Task GetMeAsync_WithValidUserId_ReturnsUser()
        {
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                FullName = "Jane Doe",
                Email = "jane@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                Role = TaskManagementAPI.Enums.UserRole.Manager
            };

            _userRepoMock.Setup(x => x.Get(userId))
                         .ReturnsAsync(user);

            var result = await _authService.GetMeAsync(userId.ToString());

            Assert.AreEqual(user.Email, result.Email);
            Assert.AreEqual(user.Role.ToString(), result.Role);
        }

        [Test]
        public void GetMeAsync_WithInvalidUserId_ThrowsUnauthorizedException()
        {
            Assert.ThrowsAsync<UnauthorizedException>(() => _authService.GetMeAsync("invalid-guid"));
        }

        [Test]
        public void GetMeAsync_UserNotFound_ThrowsNotFoundException()
        {
            var userId = Guid.NewGuid();
            _userRepoMock.Setup(x => x.Get(userId))
                         .ReturnsAsync((User)null!);

            Assert.ThrowsAsync<NotFoundException>(() => _authService.GetMeAsync(userId.ToString()));
        }
    }
}
