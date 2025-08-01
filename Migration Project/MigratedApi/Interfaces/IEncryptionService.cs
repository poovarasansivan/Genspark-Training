using MigratedApi.Models;

namespace MigratedApi.Interfaces
{
    public interface IEncryptionService
    {
        Task<Encrypt> EncryptData(Encrypt data);
        Task<bool> VerifyPassword(string plainPassword, string hashedPassword);
    }
}