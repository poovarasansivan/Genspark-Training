using System;
using TaskManagementAPI.Enums;

namespace TaskManagementAPI.Models
{
    public class TaskStatusLog
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid TaskItemId { get; set; }
        public TaskItem TaskItem { get; set; }  = null!;

        public TaskState PreviousStatus { get; set; }
        public TaskState NewStatus { get; set; }

        public Guid ChangedById { get; set; }
        public User ChangedBy { get; set; } = null!;

        public DateTime ChangedAt { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);

        
    }
}
