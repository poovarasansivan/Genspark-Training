using FitnessTracking.Models;
using System.ComponentModel.DataAnnotations;

namespace FitnessTracking.Models.DTOs
{
    public class CoachMappingRequestDto
    {
        [Required]
        public Guid CoachId { get; set; }

        [Required]
        public Guid ClientId { get; set; }
    }
}