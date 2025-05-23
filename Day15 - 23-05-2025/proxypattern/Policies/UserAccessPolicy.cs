using System;
using ProxyPattern.Interfaces;
using ProxyPattern.Models;
using ProxyPattern.Services;

namespace ProxyPattern.Policies
{
    public class UserAccessPolicy : IAccessPolicy
    {
        public void GrantAccess(RealFile file, User user)
        {
            Console.WriteLine($"User : {user.Username} | Role: {user.Role}");
            Console.WriteLine($"[User Access] Only allowed to see metadata only.");
            var metadata = file.GetMetadata();
            Console.WriteLine($"File: {metadata.FileName}, Size: {metadata.SizeInBytes} bytes");
        }
    }
}