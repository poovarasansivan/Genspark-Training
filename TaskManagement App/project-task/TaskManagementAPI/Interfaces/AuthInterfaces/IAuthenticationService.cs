using TaskManagementAPI.DTOs.Auth;

namespace TaskManagementAPI.Interfaces.Auth
{
    public interface IAuthenticationService
    {
        Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
        Task<LoginResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request);
        Task LogoutAsync(string userId); 
        Task<MeResponseDto> GetMeAsync(string userId);
    }
}
