using FitnessTracking.Models;

namespace FitnessTracking.Models.DTOs
{
    public class UserStatusBasedFilterDto : PaginationParameterDto
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
        public bool? IsActive { get; set; }
    }
}