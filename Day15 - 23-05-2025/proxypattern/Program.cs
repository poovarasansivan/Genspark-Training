using System;
using ProxyPattern.Models;
using ProxyPattern.Services;

/* Proxy is a structural design pattern that lets you provide a substitute or placeholder for another 
object. A proxy controls access to the original object, allowing you to perform something either 
before or after the request gets through to the original object.
*/

namespace ProxyPattern
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter user name:");
            string? name = Console.ReadLine()?.Trim();

            Console.WriteLine("Enter user role (admin/user/guest):");
            string? role = Console.ReadLine()?.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(role))
            {
                Console.WriteLine("Invalid input. Username and role are required.");
                return;
            }

            var user = new User { Username = name, Role = role };

            var realFilePath = @"C:\Training\proxypattern\Data\data.txt";
            var realFile = new RealFile(realFilePath);

            var proxyFile = new ProxyFile(realFile, user);

            Console.WriteLine("\nAttempting to read file...");
            proxyFile.Read(user);

            Console.WriteLine("\nExiting...");

        }
    }
}
