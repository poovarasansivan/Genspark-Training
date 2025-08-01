using System.ComponentModel.DataAnnotations;
using TaskManagementAPI.Enums;
using System.Text.Json.Serialization;

namespace TaskManagementAPI.DTOs.Users
{
    public class UserRequestDto
    {
        [Required]
        public string FullName { get; set; } = null!;

        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;

        [Required]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public UserRole Role { get; set; }
    }
}