using System;
using TaskManagementAPI.Enums;
using TaskManagementAPI.DTOs.StatusLog;
namespace TaskManagementAPI.DTOs.TaskItems
{
    public class TaskItemResponseDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public TaskState Status { get; set; }
        public DateTime? DueDate { get; set; }

        public Guid CreatedById { get; set; }
        public Guid? UpdatedById { get; set; }
        public Guid? AssignedToId { get; set; }
        public string? AssignedToName { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        //public List<string> FileNames { get; set; } = new();
        //public List<TaskStatusLogResponseDto> StatusLogs { get; set; } = new();

    }
}