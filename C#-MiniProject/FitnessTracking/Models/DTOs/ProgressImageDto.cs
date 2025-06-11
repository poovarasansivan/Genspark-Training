using FitnessTracking.Models;

namespace FitnessTracking.Models.DTOs
{
    public class ProgressImageDto
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; } = string.Empty;
        public string DownloadUrl { get; set; } = string.Empty;
    }
}