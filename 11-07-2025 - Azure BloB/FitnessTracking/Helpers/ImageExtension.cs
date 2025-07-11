namespace FitnessTracking.Helpers
{
    public static class ImageExtension
    {
        public static string GetExtension(string contentType)
        {
            return contentType switch
            {
                "image/jpeg" => "jpg",
                "image/png" => "png",
                "image/gif" => "gif",
                "image/bmp" => "bmp",
                "image/webp" => "webp",
                _ => "bin"
            };
        }
    }
}