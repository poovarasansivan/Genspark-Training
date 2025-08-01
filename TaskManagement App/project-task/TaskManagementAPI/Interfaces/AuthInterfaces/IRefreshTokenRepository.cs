using TaskManagementAPI.Models;

namespace TaskManagementAPI.Interfaces.Auth
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetById(Guid id);
        Task<RefreshToken?> GetByToken(string token);
        Task<IEnumerable<RefreshToken>> GetAllByUserId(Guid userId);
        Task Add(RefreshToken token);
        Task Update(RefreshToken token);
    }
}
