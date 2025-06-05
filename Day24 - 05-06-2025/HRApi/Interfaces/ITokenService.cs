using HRApi.Models;

namespace HRApi.Interfaces
{
    public interface ITokenService
    {
        public Task<string> GenerateToken(User user);
    }
}