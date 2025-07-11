using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessTracking.Models
{
    public class UserWorkOutPlanModel
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public UserModel? User { get; set; }

        public Guid WorkOutPlanId { get; set; }
        public WorkOutPlanModel? WorkOutPlan { get; set; }
        public Guid? UserWorkOutTaskId { get; set; }
        
        [ForeignKey(nameof(UserWorkOutTaskId))]
        public UserWorkOutTask? UserWorkOutTask { get; set; }
        public string? IsCompleted { get; set; } = "Not Completed";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}