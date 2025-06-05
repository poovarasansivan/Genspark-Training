using HRApi.Models;
using HRApi.Models.DTOs.FileHandlingDtos;

namespace HRApi.Interfaces
{
    public interface IUserService
    {
        public Task<User> GetUserByName(string Name);
        public Task<User> AddUser(UserAddRequestDto user);

    }
}