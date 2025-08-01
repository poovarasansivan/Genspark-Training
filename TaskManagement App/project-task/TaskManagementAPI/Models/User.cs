using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TaskManagementAPI.Enums;
using TaskManagementAPI.Models.Base;

namespace TaskManagementAPI.Models
{
    public class User : AuditBase
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public required string FullName { get; set; }

        [Required, EmailAddress]
        public required string Email { get; set; }

        [Required]
        public required string PasswordHash { get; set; }

        [Required]
        public UserRole Role { get; set; }

        //public string? ProfilePicturePath { get; set; }

        public ICollection<TaskItem> AssignedTasks { get; set; } = new List<TaskItem>();
        public ICollection<TaskItem> CreatedTasks { get; set; } = new List<TaskItem>();
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
