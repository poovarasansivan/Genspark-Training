namespace MigratedApi.Models.Dtos
{
    public class CartPaginationDto
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
    }
}