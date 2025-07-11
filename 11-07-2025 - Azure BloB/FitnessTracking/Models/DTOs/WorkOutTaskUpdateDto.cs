namespace FitnessTracking.Models.DTOs
{
    public class WorkOutTaskUpdate()
    {
        public Guid Id { get; set; }
        public DateTime CompletedDate { get; set; } = DateTime.UtcNow;
        public bool IsCompleted { get; set; } = false;
    }

}