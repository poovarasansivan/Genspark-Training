using ProxyPattern.Services;
using ProxyPattern.Models;

namespace ProxyPattern.Interfaces
{
    public interface IAccessPolicy
    {
        void GrantAccess(RealFile file, User user);
    }
}
