using FitnessTracking.Models.DTOs;

namespace FitnessTracking.Interfaces
{
    public interface IAuthenticationService
    {
        public Task<UserLoginResponseDto> Login(UserLoginRequestDto user);
    }
}