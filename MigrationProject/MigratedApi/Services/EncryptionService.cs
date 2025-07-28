using BCrypt.Net;
using MigratedApi.Interfaces;
using MigratedApi.Models;

namespace MigratedApi.Services
{
    public class EncryptionService : IEncryptionService
    {

        public Task<Encrypt> EncryptData(Encrypt data)
        {
            if (string.IsNullOrEmpty(data.Data))
                throw new ArgumentException("Data cannot be null or empty");

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(data.Data);
            return Task.FromResult(new Encrypt
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