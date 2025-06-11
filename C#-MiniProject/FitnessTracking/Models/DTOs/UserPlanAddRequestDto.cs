using FitnessTracking.Models;

namespace FitnessTracking.Models.DTOs
{
    public class UserPlanAddRequestDto
    {
        public Guid UserId { get; set; }
        public Guid WorkOutPlanId { get; set; }
        public string? IsCompleted { get; set; } = "Not Completed";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}