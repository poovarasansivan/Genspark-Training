namespace FitnessTracking.Models.DTOs
{
    public class ApiErrorResponseDto
    {
        public bool Success { get; set; } = false;
        public string Message { get; set; } = string.Empty;
        public object? Data { get; set; } = null;
        public Dictionary<string,string[]>? Errors { get; set; }

    }
}