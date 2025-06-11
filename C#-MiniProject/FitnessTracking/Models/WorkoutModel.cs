using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessTracking.Models
{
    public class WorkoutModel
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [ForeignKey("UserId")]
        public UserModel? User { get; set; }

        [Required]
        public Guid WorkOutPlanId { get; set; }

        [ForeignKey(nameof(WorkOutPlanId))]
        public WorkOutPlanModel? WorkOutPlan { get; set; }

        public string Type { get; set; } = string.Empty;

        public DateTime Date { get; set; } = DateTime.UtcNow;

        public TimeSpan Duration { get; set; } = TimeSpan.Zero;

        public int CaloriesBurned { get; set; }

        public string Notes { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}
