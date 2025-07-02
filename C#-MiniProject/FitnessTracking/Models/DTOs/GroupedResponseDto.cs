namespace FitnessTracking.Models.DTOs
{
    public class GroupedResponseDto
    {
        public Guid Id { get; set; }
        public Guid UserWorkOutPlanId { get; set; }
        public string PlanName { get; set; } = string.Empty;
        public string PlanDescription { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        // Coach Info
        public Guid CoachId { get; set; }
        public string CoachName { get; set; } = string.Empty;
        public string CoachEmail { get; set; } = string.Empty;

        // Client Info (User)
        public Guid ClientId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string ClientEmail { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;

        // Status Info
        public string IsCompleted { get; set; } = "Not Completed";
    }
}
