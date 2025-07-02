using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using FitnessTracking.Interfaces;
using FitnessTracking.notification;
using Castle.Core.Smtp;
using FitnessTracking.Contexts;
using Microsoft.EntityFrameworkCore;

namespace FitnessTracking.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IEmailService _emailService;
        private readonly FitnessContext _context;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(
            IHubContext<NotificationHub> hubContext,
            IEmailService emailService,
            FitnessContext context,
            ILogger<NotificationService> logger)
        {
            _hubContext = hubContext;
            _emailService = emailService;
            _context = context;
            _logger = logger;
        }

        public async Task SendNotificationAsync(string userId, string message, bool sendEmail)
        {
            _logger.LogInformation($"Sending notification to user {userId}: {message}");

            // Send SignalR real-time notification
            await _hubContext.Clients.Group(userId).SendAsync("ReceiveNotification", message);
            _logger.LogInformation($"SignalR notification sent to user {userId}");

            if (sendEmail)
            {
                var userGuid = Guid.Parse(userId);

                // Lookup user email
                var userEmail = await _context.Users
                    .Where(u => u.Id == userGuid)
                    .Select(u => u.Email)
                    .FirstOrDefaultAsync();

                if (string.IsNullOrEmpty(userEmail))
                {
                    _logger.LogWarning($"No email found for user {userId}");
                    return;
                }

                string emailHtml = $@"
                    <html>
                        <body style='font-family: Arial, sans-serif;'>
                            <h2 style='color: #2E86C1;'>Fitness Tracking Notification</h2>
                            <p>Dear User,</p>
                            <p>{message}</p>
                            <p style='margin-top:20px;'>Best regards,<br/>Fitness Tracking Team</p>
                        </body>
                    </html>
                ";

                await _emailService.SendEmailAsync(
                    userEmail,
                    "You Have a New Notification",
                    emailHtml
                );

                _logger.LogInformation($"Email notification sent to {userEmail}");
            }
        }

    }
}