using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using TaskManagementAPI.Enums;
using TaskManagementAPI.Models;
using TaskManagementAPI.Services;

namespace TaskManagementAPI.Tests.Services
{
    [TestFixture]
    public class TokenServiceTests
    {
        private TokenService _tokenService;
        private IConfiguration _configuration;

        [SetUp]
        public void Setup()
        {
            var inMemorySettings = new Dictionary<string, string>
            {
                { "Jwt:Key", "ThisIsASecretKeyForTestingPurposesOnly123!" },
                { "Jwt:Issuer", "TestIssuer" },
                { "Jwt:Audience", "TestAudience" }
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _tokenService = new TokenService(_configuration);
        }

        [Test]
        public async Task GenerateAccessToken_ValidUser_ReturnsValidToken()
        {
            // Arrange
            var user = new User
            {
                Id = Guid.NewGuid(),
                FullName = "Test User",
                Email = "test@example.com",
                Role = UserRole.TeamMember,
                PasswordHash = "dummyHash" // Required field
            };

            // Act
            var token = await _tokenService.GenerateAccessToken(user);
            var principal = _tokenService.GetPrincipalFromExpiredToken(token);
            var extractedUserId = principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Assert
            Assert.IsNotNull(token);
            Assert.IsNotNull(principal);
            Assert.AreEqual(user.Id.ToString(), extractedUserId);
        }

        [Test]
        public async Task GenerateRefreshToken_ReturnsBase64String()
        {
            // Act
            var refreshToken = await _tokenService.GenerateRefreshToken();

            // Assert
            Assert.IsNotNull(refreshToken);
            Assert.DoesNotThrow(() => Convert.FromBase64String(refreshToken));
        }

        [Test]
        public void GetPrincipalFromExpiredToken_InvalidToken_ReturnsNull()
        {
            // Arrange
            var invalidToken = "invalid.token.value";

            // Act
            var principal = _tokenService.GetPrincipalFromExpiredToken(invalidToken);

            // Assert
            Assert.IsNull(principal);
        }
    }
}
