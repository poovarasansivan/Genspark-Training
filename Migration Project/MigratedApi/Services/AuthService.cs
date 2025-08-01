using MigratedApi.Models.Dtos;
using MigratedApi.Interfaces;
using MigratedApi.Services;
using MigratedApi.Misc;
using MigratedApi.Contexts;
using MigratedApi.Repositories;
using MigratedApi.Models;

namespace MigratedApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly IEncryptionService _encryptionService;

        public AuthService(UserRepository userRepository, ITokenService tokenService, IEncryptionService encryptionService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _encryptionService = encryptionService;
        }

        public async Task<UserLoginResponseDto> Login(UserLoginRequestDto user)
        {
            var dbUser = (await _userRepository.GetAllAsync()).FirstOrDefault(u => u.Username == user.Username);
            if (dbUser == null)
            {
                throw new KeyNotFoundException("User not found.");
            }
            if (string.IsNullOrEmpty(dbUser.Password) || !await _encryptionService.VerifyPassword(user.Password!, dbUser.Password!))
            {
                throw new UnauthorizedAccessException("Invalid password.");
            }
            (string accessToken, string refreshToken) = await _tokenService.GenerateTokens(dbUser);
            return new UserLoginResponseDto
            {
                Username = user.Username,
                Token = accessToken,
                RefreshToken = refreshToken
            };
        }
    }
}