namespace MigratedApi.Models
{
    public class Model
    {
        public int ModelId { get; set; }
        public string? ModelName { get; set; }

        public ICollection<Product>? Product { get; set; }
    }

}