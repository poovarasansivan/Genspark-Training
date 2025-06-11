using FitnessTracking.Contexts;
using FitnessTracking.Interfaces;
using FitnessTracking.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using FitnessTracking.Interfaces;

namespace FitnessTracking.Services
{
    public class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey? _securityKey;

        public TokenService(IConfiguration configuration)
        {
            _securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration["Keys:JwtTokenKey"] ?? throw new ArgumentNullException("JwtTokenKey not configured"))
            );
        }

        public Task<(string AccessToken, string RefreshToken)> GenerateTokens(UserModel user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Email),
                new Claim(ClaimTypes.Role, user.Role ?? "User")
            };

            var creds = new SigningCredentials(_securityKey, SecurityAlgorithms.HmacSha256Signature);
            var tokenHandler = new JwtSecurityTokenHandler();

            var accessTokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = creds
            };

            var accessToken = tokenHandler.CreateToken(accessTokenDescriptor);
            string accessTokenString = tokenHandler.WriteToken(accessToken);

            var refreshTokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = creds
            };

            var refreshToken = tokenHandler.CreateToken(refreshTokenDescriptor);
            string refreshTokenString = tokenHandler.WriteToken(refreshToken);

            return Task.FromResult((accessTokenString, refreshTokenString));
        }

        public string GenerateAccessToken(UserModel user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Email),
                new Claim(ClaimTypes.Role, user.Role ?? "User")
            };

            var creds = new SigningCredentials(_securityKey, SecurityAlgorithms.HmacSha256Signature);
            var tokenHandler = new JwtSecurityTokenHandler();

            var accessTokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = creds
            };

            var accessToken = tokenHandler.CreateToken(accessTokenDescriptor);
            return tokenHandler.WriteToken(accessToken);
        }

        public SymmetricSecurityKey GetSecurityKey() => _securityKey!;
    }
}