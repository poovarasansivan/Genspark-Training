namespace HRApi.Models;

public class NotificationModel
{
    public int Id { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? DownloadUrl { get; set; }
}