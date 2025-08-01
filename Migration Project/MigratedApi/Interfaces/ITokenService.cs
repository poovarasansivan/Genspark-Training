using MigratedApi.Models;
using Microsoft.IdentityModel.Tokens;

namespace MigratedApi.Interfaces
{
    public interface ITokenService
    {
        public Task<(string AccessToken, string RefreshToken)> GenerateTokens(User user);
        string GenerateAccessToken(User user);
        public SymmetricSecurityKey GetSecurityKey();
    }
}