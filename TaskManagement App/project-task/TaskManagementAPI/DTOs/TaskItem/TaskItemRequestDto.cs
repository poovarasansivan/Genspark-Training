using System;
using System.ComponentModel.DataAnnotations;
using TaskManagementAPI.Enums;

namespace TaskManagementAPI.DTOs.TaskItems
{
    public class TaskItemRequestDto
    {
        [Required]
        public string Title { get; set; } = null!;

        [Required]
        public string? Description { get; set; }

        [Required]
        public TaskState Status { get; set; } = TaskState.ToDo;

        public DateTime? DueDate { get; set; }
        [Required]
        public Guid? AssignedToId { get; set; }
    }
}