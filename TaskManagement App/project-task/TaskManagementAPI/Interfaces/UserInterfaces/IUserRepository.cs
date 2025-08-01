using TaskManagementAPI.Models;

namespace TaskManagementAPI.Interfaces
{
    public interface IUserRepository : IRepository<Guid, User>
    {
        Task<User?> GetByEmail(string email);
    }
}
