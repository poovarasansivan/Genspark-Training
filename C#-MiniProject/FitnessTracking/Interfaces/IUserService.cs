using FitnessTracking.Models;
using FitnessTracking.Models.DTOs;
using FitnessTracking.Misc;

namespace FitnessTracking.Interfaces
{
    public interface IUserService
    {
        Task<UserResponseDto?> GetUserByIdAsync(Guid id);
        Task<IEnumerable<UserResponseDto>> GetAllUsersAsync();
        Task AddUserAsync(UserRegisterDto user);
        Task UpdateUserAsync(Guid id, UpdateUserDto updateUserDto);
        Task DeleteUserAsync(Guid id);
        Task UpdatePasswordAsync(Guid id, UpdatePasswordDto updatePasswordDto);
        Task UpdateUserStatusAsyn(Guid id, UpdateUserStatusDto updateUserStatusDto);
        Task<PaginatedResult<UserResponseDto>> GetPaginatedAllUsersAsync(PaginationParameterDto paginationParameterDto);
    }
}