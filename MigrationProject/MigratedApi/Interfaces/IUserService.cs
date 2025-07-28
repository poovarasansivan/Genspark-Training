using MigratedApi.Models;
using MigratedApi.Models.Dtos;

namespace MigratedApi.Interfaces
{
    public interface IUserService
    {
        Task<UserResponseDto> GetUserByIdAsync(int id);
        Task AddUserAsync(UserAddRequestDto user);
        Task UpdateUserAsync(UserAddRequestDto user);
        Task DeleteUserAsync(int id);
    }
}