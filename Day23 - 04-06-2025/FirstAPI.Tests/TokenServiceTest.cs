using NUnit.Framework;
using Microsoft.Extensions.Configuration;
using FirstAPI.Services;
using FirstAPI.Models;
using FirstAPI.Interfaces;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FirstAPI.Test
{
    public class TokenServiceTest
    {
        private ITokenService _tokenService; 

        [SetUp]
        public void Setup()
        {
            var inMemorySettings = new Dictionary<string, string> {
                {"Keys:JwtTokenKey", "ThisIsASecretKeyForJwtToken123ThisIsASecretKeyForJwtToken123!"}
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _tokenService = new TokenService(configuration);
        }

        [Test]
        public async Task GenerateToken_ValidUser_ReturnsValidJwtToken()
        {
            // Arrange
            var user = new User
            {
                Username = "poovarasan@gmail.com",
                Role = "Patient"
            };

            // Act
            var token = await _tokenService.GenerateToken(user);

            // Assert
            Assert.That(token, Is.Not.Null.And.Not.Empty);

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var usernameClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "username");
            var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "role");

            Assert.That(usernameClaim, Is.Not.Null);
            Assert.That(usernameClaim.Value, Is.EqualTo("poovarasan@gmail.com"));

            Assert.That(roleClaim, Is.Not.Null);
            Assert.That(roleClaim.Value, Is.EqualTo("Patient"));
        }
    }
}
