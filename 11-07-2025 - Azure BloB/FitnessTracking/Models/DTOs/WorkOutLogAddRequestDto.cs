using FitnessTracking.Models;

namespace FitnessTracking.Models.DTOs
{
    public class WorkOutLogAddRequestDto
    {
        public Guid UserId { get; set; }
        public Guid WorkOutPlanId { get; set; }
        public string? Type { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public TimeSpan Duration { get; set; } = TimeSpan.Zero;
        public int CaloriesBurned { get; set; }
        public string Notes { get; set; } = string.Empty;

    }
}