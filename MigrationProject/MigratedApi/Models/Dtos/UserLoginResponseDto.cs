namespace MigratedApi.Models.Dtos
{
    public class UserLoginResponseDto
    {
        public string Username { get; set; } = string.Empty;
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
    }
}