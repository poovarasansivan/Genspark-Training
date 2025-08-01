using System.ComponentModel.DataAnnotations;
using TaskManagementAPI.Enums;
namespace TaskManagementAPI.DTOs.Users
{
    public class UserUpdateDto
    {
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public UserRole? Role { get; set; }
    }
}