using Api.Models;
using Api.Models.DTOs;

namespace Api.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserModel>> GetAllUsersAsync();
        Task AddUserAsync(UserAddRequestDto user);
    }
}