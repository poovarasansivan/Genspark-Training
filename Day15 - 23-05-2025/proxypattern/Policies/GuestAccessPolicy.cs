using System;
using ProxyPattern.Interfaces;
using ProxyPattern.Models;
using ProxyPattern.Services;

namespace ProxyPattern.Policies
{
    public class GuestAccessPolicy : IAccessPolicy
    {
        public void GrantAccess(RealFile file, User user)
        {
            Console.WriteLine($"User : {user.Username} | Role: {user.Role}");
            Console.WriteLine($"[Access Denied] Does not have permission to read this file.");
        }
    }
}