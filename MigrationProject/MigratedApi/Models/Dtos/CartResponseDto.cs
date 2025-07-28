namespace MigratedApi.Models.Dtos
{
    public class CartResponseDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? ProductName { get; set; }
        public string? Image { get; set; }
        public double TotalPrice => Quantity * Price;
    }
}