namespace ElearnApi.Model.DTOs
{
    public class UploadVideoDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime UploadDate { get; set; }
        public string Category { get; set; }
    }
}