using ProxyPattern.Interfaces;
using ProxyPattern.Policies;

namespace ProxyPattern.Policies
{
    public static class AccessPolicyFactory
    {
        public static IAccessPolicy GetPolicy(string role)
        {
            return role.ToLower() switch
            {
                "admin" => new AdminAccessPolicy(),
                "user" => new UserAccessPolicy(),
                _ => new GuestAccessPolicy()
            };
        }
    }
}