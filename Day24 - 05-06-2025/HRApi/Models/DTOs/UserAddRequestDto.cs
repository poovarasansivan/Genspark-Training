namespace HRApi.Models.DTOs.FileHandlingDtos
{
    public class UserAddRequestDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Role { get; set; }
        public string? Status { get; set; }
    }
}