using System.Security.Claims;
using TaskManagementAPI.Models;

namespace TaskManagementAPI.Interfaces.Auth
{
    public interface ITokenService
    {
        Task<string> GenerateAccessToken(User user);
        Task<string> GenerateRefreshToken();
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    }
}
