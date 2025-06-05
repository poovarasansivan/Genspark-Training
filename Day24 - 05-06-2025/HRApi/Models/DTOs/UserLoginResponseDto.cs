using System.ComponentModel.DataAnnotations;

namespace HRApi.Models.DTOs.FileHandlingDtos
{
    public class UserLoginResponse
    {
        public string Name { get; set; } = string.Empty;
        public string? Token { get; set; }
    }
}