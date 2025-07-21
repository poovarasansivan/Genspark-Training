namespace ElearnApi.Models
{
    public class VideoModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime UploadDate { get; set; }
        public string BlobUrl { get; set; }
        public string Category { get; set; }
        public string ThumbnailUrl { get; set; }
    }
}