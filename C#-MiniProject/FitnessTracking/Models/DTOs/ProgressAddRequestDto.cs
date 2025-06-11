using FitnessTracking.Models;

namespace FitnessTracking.Models.DTOs
{
    public class ProgressAddRequestDto
    {
        public Guid UserId { get; set; }
        public Guid WorkOutPlanId { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public double Weight { get; set; }
        public double BodyFatPercentage { get; set; }
        public double MuscleMass { get; set; }
        public double WaterPercentage { get; set; }
        public string Notes { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}