using System;

namespace TaskManagementAPI.Models.Base
{
    public abstract class AuditBase
    {
        public Guid CreatedById { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);

        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedAt { get; set; } = null;
        public bool IsDeleted { get; set; } = false;
    }
}
