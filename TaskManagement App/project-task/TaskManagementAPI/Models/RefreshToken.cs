using System.ComponentModel.DataAnnotations;

namespace TaskManagementAPI.Models
{
    public class RefreshToken 
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public string Token { get; set; } = null!;

        public DateTime ExpiresAt { get; set; }

        public bool IsRevoked { get; set; } = false;

        public User User { get; set; } = null!;
    }
}
