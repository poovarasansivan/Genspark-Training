using FitnessTracking.Models;
using FitnessTracking.Services;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace FitnessTracking.Tests
{
    public class TokenServiceTests
    {
        private IConfiguration _configuration;
        private TokenService _tokenService;

        [SetUp]
        public void Setup()
        {
            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string> {
                    {"Keys:JwtTokenKey", "superSecretKeyForTestingPurposes"}
                }).Build();

            _tokenService = new TokenService(_configuration);
        }

        [Test]
        public void Constructor_ThrowsArgumentNullException_WhenJwtTokenKeyNotConfigured()
        {
            var config = new ConfigurationBuilder().Build();
            Assert.Throws<ArgumentNullException>(() => new TokenService(config));
        }

        [Test]
        public async Task GenerateTokens_ReturnsAccessTokenAndRefreshToken()
        {
            var user = new UserModel { Email = "test@example.com", Role = "Admin" };
            var (accessToken, refreshToken) = await _tokenService.GenerateTokens(user);

            Assert.That(!string.IsNullOrEmpty(accessToken));
            Assert.That(!string.IsNullOrEmpty(refreshToken));
        }

        [Test]
        public void GenerateAccessToken_ReturnsAccessToken()
        {
            var user = new UserModel { Email = "test@example.com", Role = "User" };
            var accessToken = _tokenService.GenerateAccessToken(user);

            Assert.That(!string.IsNullOrEmpty(accessToken));
        }

        [Test]
        public void GetSecurityKey_ReturnsSecurityKey()
        {
            SymmetricSecurityKey key = _tokenService.GetSecurityKey();
            Assert.That(key != null);
        }
    }
}