using FitnessTracking.Models;

namespace FitnessTracking.Models.DTOs
{
    public class CoachMappingResponseDto
    {
        public Guid Id { get; set; }
        public Guid CoachId { get; set; }
        public string CoachName { get; set; } = string.Empty;
        public string CoachEmail { get; set; } = string.Empty;

        public Guid ClientId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string ClientEmail { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}
