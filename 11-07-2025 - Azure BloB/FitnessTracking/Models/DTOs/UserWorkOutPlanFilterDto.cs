using FitnessTracking.Models;

namespace FitnessTracking.Models.DTOs
{
    public class UserWorkOutPlanFilterDto
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public Guid? WorkOutPlanId { get; set; }
        public string? IsCompleted { get; set; }
        public string? SortBy { get; set; }
        public string SortDirection { get; set; } = "asc";
    }
}