namespace MigratedApi.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string? Name { get; set; }

        public ICollection<Product>? Product { get; set; }
    }
}