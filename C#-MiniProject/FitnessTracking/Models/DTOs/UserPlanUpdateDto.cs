using FitnessTracking.Models;

namespace FitnessTracking.Models.DTOs
{
    public class UserPlanUpdateDto
    {
        public string? IsCompleted { get; set; } = "Not Completed";
        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}