using System;

namespace TaskManagementAPI.Models
{
    public class TaskFile
    {
        public Guid Id { get; set; }
        public Guid TaskItemId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public byte[] Data { get; set; } = Array.Empty<byte>();
        //public string FileUrl { get; set; } = string.Empty;

        public DateTime UploadedAt { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);

        public TaskItem? TaskItem { get; set; }
    }
}
