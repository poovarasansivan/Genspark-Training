namespace FitnessTracking.Models.DTOs
{
    public class WorkOutTaskAddRequestDto
    {
        public Guid UserId { get; set; }
        public Guid CoachId { get; set; }
        public Guid PlanId { get; set; }
        public string ExerciseName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Reps { get; set; }
        public int Sets { get; set; }
        public double? Weight { get; set; }
        public DateTime ScheduledDate { get; set; } = DateTime.UtcNow;
        public DateTime CompletedDate { get; set; } = DateTime.UtcNow;
    }
}