namespace MigratedApi.Models.Dtos
{
    public class OrderRequestDto
    {
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public string? Status { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public string? PaymentStatus { get; set; }
        public string? PaymentMethod { get; set; }
        public DateTime OrderDate { get; set; }
        public int Quantity { get; set; }
    }
}
