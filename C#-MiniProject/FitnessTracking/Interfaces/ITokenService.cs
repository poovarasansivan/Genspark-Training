using FitnessTracking.Models;
using Microsoft.IdentityModel.Tokens;

namespace FitnessTracking.Interfaces
{
    public interface ITokenService
    {
        public Task<(string AccessToken, string RefreshToken)> GenerateTokens(UserModel user);
        string GenerateAccessToken(UserModel user);
         public SymmetricSecurityKey GetSecurityKey();
    }
}