using FitnessTracking.Models;

namespace FitnessTracking.Models.DTOs
{
    public class UpdatePasswordDto
    {
        public string? Token { get; set; }
        public string? Password { get; set; }
    }
}