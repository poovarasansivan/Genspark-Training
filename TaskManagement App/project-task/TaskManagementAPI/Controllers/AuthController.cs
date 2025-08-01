using Microsoft.AspNetCore.Mvc;
using TaskManagementAPI.DTOs.Auth;
using TaskManagementAPI.Interfaces.Auth;
using TaskManagementAPI.CustomExceptions;
using Microsoft.AspNetCore.Authorization;
using TaskManagementAPI.ApiResponses;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace TaskManagementAPI.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticationService _authService;

        public AuthController(IAuthenticationService authService)
        {
            _authService = authService;
        }

        // POST: /api/v1/auth/login
        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<LoginResponseDto>>> Login([FromBody] LoginRequestDto loginDto)
        {
            try
            {
                var result = await _authService.LoginAsync(loginDto);
                return Ok(ApiResponse<LoginResponseDto>.SuccessResponse(result, "Login successful"));
            }
            catch (UnauthorizedException ex)
            {
                return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message, new Dictionary<string, List<string>>()));
            }
            
        }

        //post : /api/v1/auth/refresh
        [HttpPost("refresh")]
        public async Task<ActionResult<ApiResponse<LoginResponseDto>>> RefreshToken([FromBody] RefreshTokenRequestDto refreshDto)
        {
            try
            {
                var result = await _authService.RefreshTokenAsync(refreshDto);
                return Ok(ApiResponse<LoginResponseDto>.SuccessResponse(result, "Token refreshed successfully"));
            }
            catch (UnauthorizedException ex)
            {
                return Unauthorized(ApiResponse<string>.ErrorResponse(ex.Message, new Dictionary<string, List<string>>()));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message, new Dictionary<string, List<string>>()));
            };
        }


        // POST: /api/v1/auth/logout
        [HttpPost("logout")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<string>>> Logout([FromBody] string refreshToken)
        {
            try
            {
                await _authService.LogoutAsync(refreshToken);
                return Ok(ApiResponse<string>.SuccessResponse("", "Logout successful"));
            }
            catch (UnauthorizedException ex)
            {
                return Unauthorized(ApiResponse<string>.ErrorResponse(ex.Message, new Dictionary<string, List<string>>()));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message, new Dictionary<string, List<string>>()));
            }
        }

        [HttpGet("me")]
        [Authorize(Roles ="Manager,TeamMember")]
        public async Task<ActionResult<ApiResponse<MeResponseDto>>> GetCurrentUser()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                throw new UnauthorizedException("Invalid user token");
            }

            var user = await _authService.GetMeAsync(userId.ToString());
            return Ok(ApiResponse<MeResponseDto>.SuccessResponse(user, "User details fetched"));
        }

    }
}
