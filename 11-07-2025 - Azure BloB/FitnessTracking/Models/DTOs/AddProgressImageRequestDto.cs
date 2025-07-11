using FitnessTracking.Models;

namespace FitnessTracking.Models.DTOs
{
    public class AddProgressImageDto
    {
        public IFormFile File { get; set; } = null!;
        public Guid ProgressId { get; set; }
    }
}