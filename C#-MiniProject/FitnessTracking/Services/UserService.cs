using FitnessTracking.Models;
using FitnessTracking.Interfaces;
using FitnessTracking.Models.DTOs;
using FitnessTracking.Repositories;
using FitnessTracking.Misc;
using FitnessTracking.Contexts;
using Microsoft.EntityFrameworkCore;

namespace FitnessTracking.Services
{
    public class UserService : IUserService
    {

        private readonly UserRepository _userRepository;
        private readonly IEncryptionService _encryptionService;

        private readonly FitnessContext _context;
        public UserService(UserRepository userRepository, IEncryptionService encryptionService, FitnessContext context)
        {
            _userRepository = userRepository;
            _encryptionService = encryptionService;
            _context = context;
        }

        public async Task AddUserAsync(UserRegisterDto userDto)
        {
            var user = new UserModel
            {
                Name = userDto.Name!,
                Email = userDto.Email!,
                Role = userDto.Role ?? "User"
            };

            var encryptedData = await _encryptionService.EncryptData(new EncryptModel
            {
                Data = userDto.Password
            });

            user.Password = encryptedData.EncryptedData!;
            await _userRepository.AddAsync(user);
        }

        public async Task DeleteUserAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new CustomeExceptionHandler("User ID could not be empty or null", 404);
            await _userRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<UserResponseDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Select(user => new UserResponseDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                IsActive = user.IsActive,
            });
        }

        public async Task<PaginatedResult<UserResponseDto>> GetPaginatedAllUsersAsync(PaginationParameterDto pagination)
        {
            if (pagination == null)
                throw new CustomeExceptionHandler("Pagination parameters could not be empty or null", 404);

            var query = _context.Users.AsQueryable();

            if (pagination.IsActive.HasValue)
            {
                query = query.Where(u => u.IsActive == pagination.IsActive.Value);
            }

            // Handle sorting
            bool isAscending = pagination.SortDirection.ToLower() == "asc";
            if (!string.IsNullOrEmpty(pagination.SortBy))
            {
                switch (pagination.SortBy.ToLower())
                {
                    case "name":
                        query = isAscending ? query.OrderBy(u => u.Name) : query.OrderByDescending(u => u.Name);
                        break;
                    case "email":
                        query = isAscending ? query.OrderBy(u => u.Email) : query.OrderByDescending(u => u.Email);
                        break;
                    case "role":
                        query = isAscending ? query.OrderBy(u => u.Role) : query.OrderByDescending(u => u.Role);
                        break;
                    case "createdat":
                        query = isAscending ? query.OrderBy(u => u.CreatedAt) : query.OrderByDescending(u => u.CreatedAt);
                        break;
                    default:
                        query = isAscending ? query.OrderBy(u => u.CreatedAt) : query.OrderByDescending(u => u.CreatedAt);
                        break;
                }
            }
            else
            {
                query = query.OrderByDescending(u => u.CreatedAt); // Default sorting
            }

            int totalCount = await query.CountAsync();

            var data = await query
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();

            return new PaginatedResult<UserResponseDto>
            {
                Data = data.Select(user => new UserResponseDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Role = user.Role,
                    IsActive = user.IsActive,
                }).ToList(),
                TotalCount = totalCount,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize
            };
        }


        public async Task<UserResponseDto?> GetUserByIdAsync(Guid id)
        {
            if (id == null)
                throw new CustomeExceptionHandler("User ID could not be empty or null", 404);
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return null;
            return new UserResponseDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                IsActive = user.IsActive
            };
        }

        public async Task UpdatePasswordAsync(Guid id, UpdatePasswordDto updatePasswordDto)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                throw new CustomeExceptionHandler("User not found", 404);
            if (!string.IsNullOrEmpty(updatePasswordDto.Password))
            {
                var encrypted = await _encryptionService.EncryptData(new EncryptModel
                {
                    Data = updatePasswordDto.Password
                });
                user.Password = encrypted.EncryptedData;
            }
            await _userRepository.UpdateAsync(user);
        }

        public async Task UpdateUserAsync(Guid id, UpdateUserDto updateUserDto)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                throw new CustomeExceptionHandler("User not found", 404);

            if (!string.IsNullOrEmpty(updateUserDto.Name)) user.Name = updateUserDto.Name;
            if (!string.IsNullOrEmpty(updateUserDto.Email)) user.Email = updateUserDto.Email;
            if (!string.IsNullOrEmpty(updateUserDto.Password))
            {
                var encrypted = await _encryptionService.EncryptData(new EncryptModel
                {
                    Data = updateUserDto.Password
                });
                user.Password = encrypted.EncryptedData;
            }

            await _userRepository.UpdateAsync(user);
        }

        public async Task UpdateUserStatusAsyn(Guid id, UpdateUserStatusDto updateUserStatusDto)
        {
            if (id == null)
                throw new CustomeExceptionHandler("User ID could not be empty or null", 404);

            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                throw new CustomeExceptionHandler("User not found", 404);
            user.IsActive = updateUserStatusDto.IsActive;
            await _userRepository.UpdateAsync(user);
        }
    }
}