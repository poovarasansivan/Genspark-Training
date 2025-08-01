namespace MigratedApi.Models.Dtos
{
    public class CartResponseDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? Username { get; set; }
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? Image { get; set; }
        public double Price { get; set; }
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public int ColorId { get; set; }
        public string? ColorName { get; set; }
        public int ModelId { get; set; }
        public string? ModelName { get; set; }
        public int Quantity { get; set; }
        public DateTime CreatedDate { get; set; }
        public double TotalPrice => Quantity * Price;
    }
}