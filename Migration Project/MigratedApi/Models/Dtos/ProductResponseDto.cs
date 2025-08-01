namespace MigratedApi.Models.Dtos
{
    public class ProductResponseDto
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? Image { get; set; }
        public string? Description { get; set; }
        public double Price { get; set; }
        public int CategoryId { get; set; }
        public string? Name { get; set; }
        public int ColorId { get; set; }
        public string? ColorName { get; set; }
        public int ModelId { get; set; }
        public string? ModelName { get; set; }

        public DateTime SellStartDate { get; set; }
        public DateTime? SellEndDate { get; set; }
        public bool IsNew { get; set; }
    }
}