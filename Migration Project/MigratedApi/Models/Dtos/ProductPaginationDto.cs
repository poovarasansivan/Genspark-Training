namespace MigratedApi.Models.Dtos
{
    public class ProductPaginationDto
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Category { get; set; }
        public string? SearchTerm { get; set; }
        public int MinPrice { get; set; }
        public int MaxPrice { get; set; }
    }
}