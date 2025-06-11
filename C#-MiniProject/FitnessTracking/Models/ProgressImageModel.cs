using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessTracking.Models
{
    public class ProgressImageModel
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid ProgressId { get; set; }

        [ForeignKey("ProgressId")]
        public ProgressModel? Progress { get; set; }

        [Required]
        public byte[] ImageData { get; set; } = Array.Empty<byte>();

        public string ContentType { get; set; } = string.Empty;

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }
}
