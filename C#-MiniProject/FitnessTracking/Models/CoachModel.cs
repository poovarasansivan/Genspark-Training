using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessTracking.Models
{
    public class CoachClientMap
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid CoachId { get; set; }

        [ForeignKey(nameof(CoachId))]
        public UserModel? Coach { get; set; } 

        [Required]
        public Guid ClientId { get; set; }

        [ForeignKey(nameof(ClientId))]
        public UserModel? Client { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
