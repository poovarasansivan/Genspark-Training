using System.ComponentModel.DataAnnotations;

namespace FitnessTracking.Models
{
    public class UserModel
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string? Password { get; set; }

        [Required]
        public string Role { get; set; } = "User";
        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation
        public ICollection<WorkoutModel> Workouts { get; set; } = new List<WorkoutModel>();
        public ICollection<UserWorkOutPlanModel> UserWorkOutPlans { get; set; } = new List<UserWorkOutPlanModel>();

    }
}
