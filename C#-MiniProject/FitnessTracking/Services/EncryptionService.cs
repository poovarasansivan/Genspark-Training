using FitnessTracking.Interfaces;
using FitnessTracking.Models;
using BCrypt.Net;

namespace FitnessTracking.Services
{
    public class EncryptionService : IEncryptionService
    {
        public Task<EncryptModel> EncryptData(EncryptModel data)
        {
            if (string.IsNullOrEmpty(data.Data))
                throw new ArgumentException("Data cannot be null or empty");

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(data.Data);
            return Task.FromResult(new EncryptModel
            {
                Data = data.Data,
                EncryptedData = hashedPassword
            });
        }

        public Task<bool> VerifyPassword(string plainPassword, string hashedPassword)
        {
            return Task.FromResult(BCrypt.Net.BCrypt.Verify(plainPassword, hashedPassword));
        }
    }
}
