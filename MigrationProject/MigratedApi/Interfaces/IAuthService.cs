using MigratedApi.Models.Dtos;

namespace MigratedApi.Interfaces
{
    public interface IAuthService
    {
        public Task<UserLoginResponseDto> Login(UserLoginRequestDto user);

    }
}
