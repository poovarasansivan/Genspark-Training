using TaskManagementAPI.DTOs.Auth;
using TaskManagementAPI.Interfaces;
using TaskManagementAPI.CustomExceptions;
using TaskManagementAPI.Interfaces.Auth;
using TaskManagementAPI.Mapper;
using BCrypt.Net;
using TaskManagementAPI.Models;

namespace TaskManagementAPI.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly AuthMapper _authMapper;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public AuthenticationService(
            IUserRepository userRepository,
            ITokenService tokenService,
            AuthMapper authMapper,
            IRefreshTokenRepository refreshTokenRepository)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _authMapper = authMapper;
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto loginDto)
        {
            var user = await _userRepository.GetByEmail(loginDto.Email);
            if (user == null)
                throw new UnauthorizedException("Invalid email or password.");

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash);
            if (!isPasswordValid)
                throw new UnauthorizedException("Invalid email or password.");

            var accessToken = await _tokenService.GenerateAccessToken(user);
            var refreshToken = await _tokenService.GenerateRefreshToken();

            var refreshTokenEntity = new RefreshToken
            {
                UserId = user.Id,
                Token = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(3),
                IsRevoked = false
            };

            await _refreshTokenRepository.Add(refreshTokenEntity);

            return _authMapper.MapToLoginResponse(user, accessToken, refreshToken);
        }

        public async Task<LoginResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request)
        {
            var existingToken = await _refreshTokenRepository.GetByToken(request.RefreshToken);
            if (existingToken == null || existingToken.IsRevoked || existingToken.ExpiresAt < DateTime.UtcNow)
                throw new UnauthorizedException("Invalid or expired refresh token.");

            var user = await _userRepository.Get(existingToken.UserId);
            if (user == null)
                throw new UnauthorizedException("User not found.");

            existingToken.IsRevoked = true;
            await _refreshTokenRepository.Update(existingToken);


            var newAccessToken = await _tokenService.GenerateAccessToken(user);
            var newRefreshToken = await _tokenService.GenerateRefreshToken();

            var newTokenEntity = new RefreshToken
            {
                UserId = user.Id,
                Token = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(3),
                IsRevoked = false
            };

            await _refreshTokenRepository.Add(newTokenEntity);

            return _authMapper.MapToLoginResponse(user, newAccessToken, newRefreshToken);
        }

        public async Task LogoutAsync(string refreshToken)
        {
            var existingToken = await _refreshTokenRepository.GetByToken(refreshToken);
            if (existingToken == null)
                throw new UnauthorizedException("Invalid refresh token.");
            if (existingToken != null && !existingToken.IsRevoked)
            {
                existingToken.IsRevoked = true;
                await _refreshTokenRepository.Update(existingToken);

            }
        }

        public async Task<MeResponseDto> GetMeAsync(string userId)
        {
            if (!Guid.TryParse(userId, out var guid) || guid == Guid.Empty)
                throw new UnauthorizedException("Invalid user token");

            var user = await _userRepository.Get(guid);
            if (user == null)
                throw new NotFoundException("User not found.");

            return _authMapper.MapUserToMeResponse(user);
        }
    }
}
