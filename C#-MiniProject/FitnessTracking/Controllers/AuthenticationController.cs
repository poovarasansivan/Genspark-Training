using Microsoft.AspNetCore.Mvc;
using FitnessTracking.Contexts;
using FitnessTracking.Models.DTOs;
using FitnessTracking.Models;
using FitnessTracking.Interfaces;
using FitnessTracking.Repositories;
using FitnessTracking.Services;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace FitnessTracking.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly ITokenService _tokenService;
        private readonly UserRepository _userRepository;
        private readonly ILogger<AuthenticationController> _logger;

        public AuthenticationController(
            IAuthenticationService authenticationService,
            ITokenService tokenService,
            UserRepository userRepository,
            ILogger<AuthenticationController> logger)
        {
            _authenticationService = authenticationService;
            _tokenService = tokenService;
            _userRepository = userRepository;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequestDto loginRequestDto)
        {
            try
            {
                _logger.LogInformation("Login attempt for email: {Email}", loginRequestDto.Email);
                var response = await _authenticationService.Login(loginRequestDto);
                _logger.LogInformation("Login successful for email: {Email}", loginRequestDto.Email);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Login failed for email: {Email}", loginRequestDto.Email);
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(RefreshTokenRequestDto refreshRequest)
        {
            if (string.IsNullOrEmpty(refreshRequest.RefreshToken))
            {
                _logger.LogWarning("Refresh token request failed: Token is missing.");
                return BadRequest("Refresh Token is Required");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken validatedToken;

            try
            {
                _logger.LogInformation("Attempting to validate refresh token.");
                var principal = tokenHandler.ValidateToken(refreshRequest.RefreshToken, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = _tokenService.GetSecurityKey(),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero,
                    ValidateLifetime = false // We manually check expiration
                }, out validatedToken);

                var jwtToken = validatedToken as JwtSecurityToken;
                if (jwtToken == null)
                {
                    _logger.LogWarning("Refresh token validation failed: Not a valid JWT token.");
                    return Unauthorized("Invalid token");
                }

                var expClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Exp);
                if (expClaim == null || !long.TryParse(expClaim.Value, out var expUnix))
                {
                    _logger.LogWarning("Refresh token validation failed: Expiration claim missing.");
                    return Unauthorized("Token missing expiration");
                }

                var expTime = DateTimeOffset.FromUnixTimeSeconds(expUnix).UtcDateTime;
                if (expTime < DateTime.UtcNow)
                {
                    _logger.LogWarning("Refresh token expired at {ExpiryTime}.", expTime);
                    return Unauthorized("Refresh token expired");
                }

                var email = principal.FindFirstValue(ClaimTypes.NameIdentifier);
                if (email == null)
                {
                    _logger.LogWarning("Refresh token validation failed: Email claim not found.");
                    return Unauthorized("Invalid Refresh Token");
                }

                var user = (await _userRepository.GetAllAsync()).FirstOrDefault(u => u.Email == email);
                if (user == null)
                {
                    _logger.LogWarning("Refresh token validation failed: No user found with email {Email}.", email);
                    return Unauthorized("User not found");
                }

                string newAccessToken = _tokenService.GenerateAccessToken(user);
                _logger.LogInformation("Refresh token validated. New access token issued for {Email}.", email);

                return Ok(new RefreshTokenResponseDto
                {
                    AccessToken = newAccessToken,
                    RefreshToken = refreshRequest.RefreshToken
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception during refresh token validation.");
                return Unauthorized($"Invalid or Expired Token: {ex.Message}");
            }
        }
    }
}
