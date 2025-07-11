using FitnessTracking.Models;

namespace FitnessTracking.Models.DTOs
{
    public class WorkOutAddRequestDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}