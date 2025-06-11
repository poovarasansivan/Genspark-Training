namespace FitnessTracking.Models.DTOs
{
    public class UserLoginResponseDto
    {
        public string? Email { get; set; }
        public string? Token { get; set; }
        public string? RefreshToken{ get; set; }
    }
}