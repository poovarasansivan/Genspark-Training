using Microsoft.AspNetCore.Mvc;
using MigratedApi.Models.Dtos;
using MigratedApi.Interfaces;
using MigratedApi.Misc;
using MigratedApi.Services;
using MigratedApi.Repositories;
using MigratedApi.Contexts;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace MigratedApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ITokenService _tokenService;
        private readonly UserRepository _userRepository;
        private readonly ShopDbContext _context;
        public AuthController(IAuthService authService, ITokenService tokenService, UserRepository userRepository, ShopDbContext context)
        {
            _authService = authService;
            _tokenService = tokenService;
            _userRepository = userRepository;
            _context = context;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequestDto userLoginRequest)
        {
            try
            {
                var response = await _authService.Login(userLoginRequest);
                return Ok(response);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiErrorResponseDto { Message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ApiErrorResponseDto { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiErrorResponseDto { Message = "An unexpected error occurred.", Data = ex.Message });
            }
        }
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRefreshRequestDto tokenRefreshRequest)
        {
            if (string.IsNullOrEmpty(tokenRefreshRequest.RefreshToken))
            {
                return BadRequest("Refresh token is required.");
            }

            var token = new JwtSecurityTokenHandler();
            SecurityToken validatedToken;

            try
            {
                var principal = token.ValidateToken(tokenRefreshRequest.RefreshToken, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = _tokenService.GetSecurityKey(),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero,
                    ValidateLifetime = false

                }, out validatedToken);

                var jwtToken = validatedToken as JwtSecurityToken;
                if (jwtToken == null)
                {
                    return Unauthorized("Invalid token");
                }

                var expClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Exp);
                if (expClaim == null || !long.TryParse(expClaim.Value, out long exp))
                {
                    return Unauthorized("Token missing expiration");
                }

                var expTime = DateTimeOffset.FromUnixTimeSeconds(exp).UtcDateTime;
                if (expTime < DateTime.UtcNow)
                {
                    return Unauthorized("Refresh Token has expired");
                }

                var userdId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userdId))
                {
                    return Unauthorized("Invalid Refresh token");
                }
                var id = int.Parse(userdId);
                var user = await _userRepository.GetByIdAsync(id);
                string? newAccessToken = _tokenService.GenerateAccessToken(user);
                return Ok(new RefreshTokenResponseDto
                {
                    AccessToken = newAccessToken,
                    RefreshToken = tokenRefreshRequest.RefreshToken
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiErrorResponseDto { Message = ex.Message });
            }

        }
    }
}