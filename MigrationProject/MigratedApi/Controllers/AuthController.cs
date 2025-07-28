using Microsoft.AspNetCore.Mvc;
using MigratedApi.Models.Dtos;
using MigratedApi.Interfaces;
using MigratedApi.Misc;
using MigratedApi.Services;
using MigratedApi.Repositories;

namespace MigratedApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ITokenService _tokenService;
        private readonly UserRepository _userRepository;
        public AuthController(IAuthService authService, ITokenService tokenService, UserRepository userRepository)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _authService = authService;
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
    }
}