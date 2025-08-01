namespace MigratedApi.Models.Dtos
{
    public class OrdersPaginationDto
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? UserName { get; set; }
        public string? Status { get; set; }
        public string? SortBy { get; set; }
        public string? SortDirection { get; set; }
        public string? SearchTerm { get; set; }
    }
}