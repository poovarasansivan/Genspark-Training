using HRApi.Interfaces;
using HRApi.Models;
using HRApi.Models.DTOs.FileHandlingDtos;

namespace HRApi.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly ITokenService _tokenService;
        private readonly IEncryptionService _encryptionService;
        private readonly IRepository<string, User> _userRepository;

        public AuthenticationService(ITokenService tokenService, IEncryptionService encryptionService,
                                     IRepository<string, User> userRepository)
        {
            _tokenService = tokenService;
            _encryptionService = encryptionService;
            _userRepository = userRepository;
        }

        public async Task<UserLoginResponse> Login(UserLoginRequest user)
        {
            var dbUser = await _userRepository.Get(user.Name);
            if (dbUser == null)
            {
                throw new Exception("No such user");
            }
            var encryptedData = await _encryptionService.EncryptData(new EncryptModel
            {
                Data = user.Password,
                HashKey = dbUser.HashKey
            });
            for (int i = 0; i < encryptedData.EncryptedData.Length; i++)
            {
                if (encryptedData.EncryptedData[i] != dbUser.Password[i])
                {
                    throw new Exception("Invalid password");
                }
            }
            var token = await _tokenService.GenerateToken(dbUser);
            return new UserLoginResponse
            {
                Name = user.Name,
                Token = token,
            };
        }
    }
}