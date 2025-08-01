
using TaskManagementAPI.Enums;
namespace TaskManagementAPI.DTOs.Users
{
    public class UserResponseDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public UserRole Role { get; set; }
        // public string? ProfilePicturePath { get; set; } // Uncomment if needed
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}