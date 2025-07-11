using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessTracking.Models
{
    public class ProgressModel
    {
        [Key]
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        
        [ForeignKey("UserId")]
        public UserModel? User { get; set; }

        [Required]
        public Guid WorkOutPlanId { get; set; }

        [ForeignKey("WorkOutPlanId")]
        public WorkOutPlanModel? WorkOutPlan { get; set; }

        public DateTime Date { get; set; } = DateTime.UtcNow;

        public double Weight { get; set; }

        public double BodyFatPercentage { get; set; }

        public double MuscleMass { get; set; }

        public double WaterPercentage { get; set; }

        public string Notes { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<ProgressImageModel>? ProgressImage { get; set; }

    }
}
