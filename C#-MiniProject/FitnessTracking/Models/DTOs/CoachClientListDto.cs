using System;
using FitnessTracking.Models;

namespace FitnessTracking.Models.DTOs
{
    public class CoachClientListDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}
