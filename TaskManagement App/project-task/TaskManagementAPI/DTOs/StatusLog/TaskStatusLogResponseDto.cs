using TaskManagementAPI.Enums;

namespace TaskManagementAPI.DTOs.StatusLog
{
    public class TaskStatusLogResponseDto
    {
        public Guid Id { get; set; }
        public TaskState PreviousStatus { get; set; }
        public TaskState NewStatus { get; set; }
        public DateTime ChangedAt { get; set; }
        public Guid ChangedById { get; set; }
        public string ChangedByName { get; set; } = null!;
    }
}
