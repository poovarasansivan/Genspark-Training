using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessTracking.Models
{
    public class WorkOutPlanModel
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public DateTime StartDate { get; set; } = DateTime.UtcNow;

        public DateTime EndDate { get; set; } = DateTime.UtcNow.AddDays(30);

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public ICollection<WorkoutModel> Workouts { get; set; } = new List<WorkoutModel>();
        public ICollection<ProgressModel> ProgressUpdates { get; set; } = new List<ProgressModel>();
    }
}
