using HRApi.Models.DTOs.FileHandlingDtos;

namespace HRApi.Interfaces
{
    public interface IAuthenticationService
    {
        public Task<UserLoginResponse> Login(UserLoginRequest user);

    }
}