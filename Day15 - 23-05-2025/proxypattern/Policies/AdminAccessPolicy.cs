using System;
using ProxyPattern.Interfaces;
using ProxyPattern.Models;
using ProxyPattern.Services;

namespace ProxyPattern.Policies
{

    public class AdminAccessPolicy : IAccessPolicy
    {
        public void GrantAccess(RealFile file, User user)
        {
            Console.WriteLine($"User : {user.Username} | Role: {user.Role}");
            Console.WriteLine($"[Admin Access] Allows to access full file content.");
            file.Read(user);
        }
    }

}