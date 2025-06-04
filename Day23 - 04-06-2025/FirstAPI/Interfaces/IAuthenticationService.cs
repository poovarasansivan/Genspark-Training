using FirstAPI.Models.DTOs.DoctorSpecialities;

namespace FirstAPI.Interfaces
{
    public interface IAuthenticationService
    {
        Task<UserLoginResponse> Loginwithgoogle(string email);
    }
}