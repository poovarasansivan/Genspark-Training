using System.ComponentModel.DataAnnotations;
using FitnessTracking.Models;
using FitnessTracking.Helpers;

namespace FitnessTracking.Models.DTOs
{
    public class UpdateUserDto
    {
        [Required]
        [MininumLength(6)]
        public string? Name { get; set; }
        [Required]
        [EmailValidator(new[] { "gmail.com", "yahoo.com" })]
        public string? Email { get; set; }

        [Required]
        [PasswordValidator]
        public string? Password { get; set; }

        public string? Role { get; set; }
    }
}