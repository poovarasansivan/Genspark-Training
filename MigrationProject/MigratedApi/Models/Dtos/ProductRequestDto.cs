namespace MigratedApi.Models.Dtos
{
    public class ProductRequestDto
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public IFormFile? Image { get; set; }
        public double Price { get; set; }
        public int CategoryId { get; set; }
        public int ColorId { get; set; }
        public int ModelId { get; set; }
        public DateTime SellStartDate { get; set; }
        public DateTime? SellEndDate { get; set; }
        public bool IsNew { get; set; }
    }
}