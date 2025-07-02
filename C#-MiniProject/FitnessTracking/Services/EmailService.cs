using Castle.Core.Smtp;
using FitnessTracking.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace FitnessTracking.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendEmailAsync(string userEmail, string subject, string message)
        {
            var smtpHost = "smtp.gmail.com";
            var smtpPort = 587;
            var smtpUser = "poovarasansivan3@gmail.com";
            var smtpPass = "zqjn tlmu myzg bchr";
            var fromEmailAddress = "poovarasansivan3@gmail.com";
            var fromDisplayName = "Fitness Tracking Application";

            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmailAddress, fromDisplayName),
                Subject = subject,
                Body = message,
                IsBodyHtml = true
            };

            mailMessage.To.Add(userEmail);

            using var smtpClient = new SmtpClient(smtpHost, smtpPort)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(smtpUser, smtpPass)
            };

            try
            {
                await smtpClient.SendMailAsync(mailMessage);
                _logger.LogInformation($"Email sent to {userEmail}");
            }
            catch (SmtpException ex)
            {
                _logger.LogError(ex, "Error sending email.");
                throw;
            }
        }

    }
}