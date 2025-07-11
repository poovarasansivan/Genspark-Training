using FitnessTracking.Models;

namespace FitnessTracking.Models.DTOs
{
    public class WorkOutLogResponseDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public Guid WorkOutPlanId { get; set; }
        public string? WorkOutPlanName { get; set; }
        public string? Type { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public TimeSpan Duration { get; set; } = TimeSpan.Zero;
        public int CaloriesBurned { get; set; }
        public string Notes { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; } = null;
    }
}
