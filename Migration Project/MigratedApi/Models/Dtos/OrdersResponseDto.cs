namespace MigratedApi.Models.Dtos
{
    public class OrdersResponseDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? UserName { get; set; }
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
        public string? Status { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public string? PaymentStatus { get; set; }
        public string? PaymentMethod { get; set; }
        public DateTime OrderDate { get; set; }
        public int Quantity { get; set; }
        public int TotalAmount { get; set; }
    }
}