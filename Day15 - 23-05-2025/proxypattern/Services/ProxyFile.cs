using ProxyPattern.Interfaces;
using ProxyPattern.Models;
using ProxyPattern.Policies;

namespace ProxyPattern.Services
{
    public class ProxyFile : IFile
    {
        private readonly RealFile _realFile;
        private readonly IAccessPolicy _policy;

        public ProxyFile(RealFile realFile, User user)
        {
            _realFile = realFile;
            _policy = AccessPolicyFactory.GetPolicy(user.Role);
        }

        public void Read(User user)
        {
            _policy.GrantAccess(_realFile, user);
        }
    }
}
