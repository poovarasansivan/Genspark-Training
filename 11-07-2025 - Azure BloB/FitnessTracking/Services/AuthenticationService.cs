using FitnessTracking.Interfaces;
using FitnessTracking.Models;
using FitnessTracking.Models.DTOs;
using FitnessTracking.Repositories;
using FitnessTracking.Misc;

namespace FitnessTracking.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserRepository? _useRepository;
        private readonly IEncryptionService? _encryptionService;
        private readonly ITokenService? _tokenService;
        private readonly ILogger<AuthenticationService>? _logger;

        public AuthenticationService(UserRepository userRepository, IEncryptionService encryptionService,
                             ITokenService tokenService, ILogger<AuthenticationService> logger)
        {
            _useRepository = userRepository;
            _encryptionService = encryptionService;
            _tokenService = tokenService;
            _logger = logger;
        }


        public async Task<UserLoginResponseDto> Login(UserLoginRequestDto user)
        {
            var dbUser = (await _useRepository.GetAllAsync()).FirstOrDefault(u => u.Email == user.Email);

            if (dbUser == null)
            {
                _logger?.LogCritical("User not found");
                throw new CustomeExceptionHandler("User not found", 404);
            }
            if (!dbUser.IsActive)
            {
                _logger?.LogError("User is not active");
                throw new CustomeExceptionHandler("User is not active", 403);
            }

            if (string.IsNullOrEmpty(dbUser.Password) || !await _encryptionService.VerifyPassword(user.Password!, dbUser.Password!))
            {
                _logger?.LogError("Invalid Login Password");
                throw new CustomeExceptionHandler("Invalid Login Password", 401);
            }

            (string accessToken, string refreshToken) = await _tokenService.GenerateTokens(dbUser);
            return new UserLoginResponseDto
            {
                Email = user.Email,
                Token = accessToken,
                RefreshToken = refreshToken
            };
        }

    }
}