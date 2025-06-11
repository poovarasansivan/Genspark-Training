using FitnessTracking.Models;

namespace FitnessTracking.Models.DTOs
{
    public class UserPlanResponseDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string? UserName { get; set; }
        public Guid WorkOutPlanId { get; set; }
        public string? WorkOutPlanName { get; set; }
        public string? IsCompleted { get; set; } = "Not Completed";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}