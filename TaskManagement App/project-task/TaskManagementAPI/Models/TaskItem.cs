using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TaskManagementAPI.Enums;
using TaskManagementAPI.Models.Base;

namespace TaskManagementAPI.Models
{
    public class TaskItem : AuditBase
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public required string Title { get; set; }

        public required string? Description { get; set; }

        public TaskState Status { get; set; } = TaskState.ToDo;

        public DateTime? DueDate { get; set; }

        public User CreatedByUser { get; set; } = null!;
        public User? UpdatedByUser { get; set; }


        public Guid? AssignedToId { get; set; }
        public User? AssignedTo { get; set; }

        public ICollection<TaskStatusLog> StatusLogs { get; set; } = new List<TaskStatusLog>();

        public ICollection<TaskFile> Files { get; set; } = new List<TaskFile>();
        }
}
