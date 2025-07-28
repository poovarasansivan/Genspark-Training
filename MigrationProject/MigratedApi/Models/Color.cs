namespace MigratedApi.Models
{
    public class Color
    {
        public int ColorId { get; set; }
        public string? ColorName { get; set; }
        public ICollection<Product>? Products { get; set; }
    }

}