namespace FitnessTracking.Models.DTOs
{
    public class WorkOutLogUpdateDto
    {
        public Guid WorkOutPlanId { get; set; }
        public string Type { get; set; } = string.Empty;
        public DateTime? Date { get; set; }
        public TimeSpan Duration { get; set; } = TimeSpan.Zero;
        public int? CaloriesBurned { get; set; }
    }
}