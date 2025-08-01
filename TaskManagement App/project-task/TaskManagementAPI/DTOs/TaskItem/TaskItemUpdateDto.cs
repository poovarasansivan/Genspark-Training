using System;
using System.ComponentModel.DataAnnotations;
using TaskManagementAPI.Enums;

namespace TaskManagementAPI.DTOs.TaskItems
{
    public class TaskItemUpdateDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public TaskState? Status { get; set; }
        public DateTime? DueDate { get; set; }
        public Guid? AssignedToId { get; set; }
    }
}