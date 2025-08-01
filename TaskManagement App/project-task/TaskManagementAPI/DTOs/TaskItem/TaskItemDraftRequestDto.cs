using System;
using System.ComponentModel.DataAnnotations;
using TaskManagementAPI.Enums;

namespace TaskManagementAPI.DTOs.TaskItems
{
    public class TaskItemDraftRequestDto
    {
        [Required]
        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public TaskState Status { get; set; } = TaskState.Draft;

        public DateTime? DueDate { get; set; }
        
        public Guid? AssignedToId { get; set; }
    }
}