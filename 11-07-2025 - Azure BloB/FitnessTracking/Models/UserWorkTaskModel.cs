using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessTracking.Models
{
    public class UserWorkOutTask
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public UserModel? User { get; set; }

        [Required]
        public Guid CoachId { get; set; }

        [ForeignKey(nameof(CoachId))]
        public UserModel? Coach { get; set; }

        [Required]
        public Guid PlanId { get; set; }

        [ForeignKey(nameof(PlanId))]
        public WorkOutPlanModel? Plan { get; set; }

        [Required]
        [MaxLength(150)]
        public string? ExerciseName { get; set; }

        public string? Description { get; set; }

        [Required]
        public int Reps { get; set; }

        [Required]
        public int Sets { get; set; }

        public double? Weight { get; set; }

        public DateTime ScheduledDate { get; set; } = DateTime.UtcNow;
        public DateTime CompletedDate { get; set; } = DateTime.UtcNow;
        public bool IsCompleted { get; set; } = false;

        public ICollection<UserWorkOutPlanModel> UserWorkOutPlans { get; set; } = new List<UserWorkOutPlanModel>();

    }
}