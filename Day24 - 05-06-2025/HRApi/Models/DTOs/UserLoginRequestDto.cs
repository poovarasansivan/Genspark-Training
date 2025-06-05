using System.ComponentModel.DataAnnotations;

namespace HRApi.Models.DTOs.FileHandlingDtos
{
     public class UserLoginRequest
    {
        [Required(ErrorMessage = "Username is manditory")]
        [MinLength(5,ErrorMessage ="Invalid entry for username")]
        public string Name { get; set; } = string.Empty;
        [Required(ErrorMessage = "Password is manditory")]
        public string Password { get; set; } = string.Empty;
    }
}