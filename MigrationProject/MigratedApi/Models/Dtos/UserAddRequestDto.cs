namespace MigratedApi.Models.Dtos
{
    public class UserAddRequestDto
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}