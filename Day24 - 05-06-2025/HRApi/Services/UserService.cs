using System;
using HRApi.Models;
using HRApi.Models.DTOs.FileHandlingDtos;
using HRApi.Interfaces;
using System.Threading.Tasks;
using AutoMapper;
using System.Collections.Generic;

namespace HRApi.Services
{
    public class UserServices : IUserService
    {
        private readonly IEncryptionService _encryptionService;
        private readonly IRepository<string, User> _userRepository;
        private readonly IMapper _mapper;

        public UserServices(IEncryptionService encryptionService, IRepository<string, User> userRepository, IMapper mapper)
        {
            _encryptionService = encryptionService;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<User> GetUserByName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Invalid user name");
            }

            return await _userRepository.Get(name);
        }

        public async Task<User> AddUser(UserAddRequestDto userDto)
        {
            try
            {
                var user = _mapper.Map<User>(userDto);

                var encryptedData = await _encryptionService.EncryptData(new EncryptModel
                {
                    Data = userDto.Password
                });

                user.Password = encryptedData.EncryptedData;
                user.HashKey = encryptedData.HashKey;
                user.Role = "User";
                user = await _userRepository.Add(user);
                return user;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while adding user: {ex.Message}");
            }
        }
    }
}
