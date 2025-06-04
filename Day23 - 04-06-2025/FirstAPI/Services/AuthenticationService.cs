using FirstAPI.Models;
using FirstAPI.Interfaces;
using FirstAPI.Models.DTOs.DoctorSpecialities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;

namespace FirstAPI.Services
{
    public class AuthenticationService : FirstAPI.Interfaces.IAuthenticationService
    {
        private readonly IRepository<string, User> _userRepository;
        private readonly ITokenService _tokenService;

        public AuthenticationService(IRepository<string, User> userRepository, ITokenService tokenService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
        }

        public async Task<UserLoginResponse> Loginwithgoogle(string email)
        {
            var users = await _userRepository.GetAll();
            var dbUser = users.FirstOrDefault(u => u.Username == email);
            if (dbUser == null)
            {
                throw new Exception("User not found");
            }

            var token = await _tokenService.GenerateToken(dbUser);
            return new UserLoginResponse
            {
                Username = dbUser.Username,
                Token = token,
            };
        }

    }
}