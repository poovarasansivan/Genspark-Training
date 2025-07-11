using FitnessTracking.Models;

namespace FitnessTracking.Models.DTOs
{
    public class ProgressResponseDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public Guid WorkOutPlanId { get; set; }
        public string WorkOutPlanName { get; set; } = string.Empty;
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public double Weight { get; set; }
        public double BodyFatPercentage { get; set; }
        public double MuscleMass { get; set; }
        public double WaterPercentage { get; set; }
        public string Notes { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<ProgressImageDto>? ProgressImage { get; set; } = new List<ProgressImageDto>();
    }
}