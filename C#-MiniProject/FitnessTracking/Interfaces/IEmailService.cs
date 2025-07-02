namespace FitnessTracking.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string userId, string subject, string message);
    }
}