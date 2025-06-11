using FitnessTracking.Models;

namespace FitnessTracking.Interfaces
{
    public interface IEncryptionService
    {
        Task<EncryptModel> EncryptData(EncryptModel data);
        Task<bool> VerifyPassword(string plainPassword, string hashedPassword);
    }
}