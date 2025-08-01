using MigratedApi.Models;
using MigratedApi.Interfaces;
using MigratedApi.Repositories;
using MigratedApi.Contexts;
using MigratedApi.Services;
using Microsoft.EntityFrameworkCore;
using MigratedApi.Models.Dtos;
using MigratedApi.Contexts;

namespace MigratedApi.Services
{
    public class UserService : IUserService
    {
        private readonly UserRepository _userRepository;
        private readonly IEncryptionService _encryptionService;
        private readonly ShopDbContext _context;

        public UserService(UserRepository userRepository, IEncryptionService encryptionService, ShopDbContext context)
        {
            _userRepository = userRepository;
            _encryptionService = encryptionService;
            _context = context;
        }

        public async Task AddUserAsync(UserAddRequestDto user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password))
                throw new ArgumentException("Username and Password cannot be null or empty");

            var encryptModel = new Encrypt { Data = user.Password };
            var encryptedResult = await _encryptionService.EncryptData(encryptModel);

            string hashedPassword = encryptedResult.EncryptedData;

            var newUser = new User
            {
                UserId = user.UserId,
                Username = user.Username,
                Password = hashedPassword
            };

            await _userRepository.AddAsync(newUser);
        }


        public async Task DeleteUserAsync(int id)
        {
            if (id == 0)
                throw new ArgumentException("User ID cannot be empty");

            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                throw new KeyNotFoundException($"User with ID {id} not found");
            await _userRepository.DeleteAsync(user.UserId);
        }

        public async Task<UserResponseDto> GetUserByIdAsync(int id)
        {
            if (id == 0)
                throw new ArgumentException("User ID cannot be empty");

            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                throw new KeyNotFoundException($"User with ID {id} not found");

            return new UserResponseDto
            {
                UserId = user.UserId,
                UserName = user.Username,
            };
        }

        public async Task UpdateUserAsync(UserAddRequestDto user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (user.UserId == 0)
                throw new ArgumentException("User ID cannot be empty");

            var existingUser = await _userRepository.GetByIdAsync(user.UserId);
            if (existingUser == null)
                throw new KeyNotFoundException($"User with ID {user.UserId} not found");

            existingUser.Username = user.Username;
            existingUser.Password = user.Password;

            if (!string.IsNullOrEmpty(user.Password))
            {
                var encryptModel = new Encrypt { Data = user.Password };
                var encryptedResult = await _encryptionService.EncryptData(encryptModel);
                existingUser.Password = encryptedResult.Data;
            }
            await _userRepository.UpdateAsync(existingUser);
        }
    }
}
