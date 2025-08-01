namespace MigratedApi.Models
{
    public class Contact
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Subject { get; set; }
        public string? Messgae { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}