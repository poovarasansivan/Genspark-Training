namespace MigratedApi.Models.Dtos
{
    public class NewsRequestDto
    {
        public int NewsId { get; set; }
        public int UserId { get; set; }
        public string? Title { get; set; }
        public string? ShortDescription { get; set; }
        public string? Content { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? Status { get; set; }
        public IFormFile? Image { get; set; }
    }
}