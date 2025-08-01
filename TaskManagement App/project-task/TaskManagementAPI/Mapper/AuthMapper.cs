using TaskManagementAPI.DTOs.Auth;
using TaskManagementAPI.Models;

namespace TaskManagementAPI.Mapper
{
    public class AuthMapper
    {
        public MeResponseDto MapUserToMeResponse(User user)
        {
            return new MeResponseDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role.ToString()
            };
        }

        public LoginResponseDto MapToLoginResponse(User user, string accessToken, string refreshToken, int expiresIn = 900)
        {
            return new LoginResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresIn = expiresIn,
                User = MapUserToMeResponse(user)
            };
        }
    }
}
