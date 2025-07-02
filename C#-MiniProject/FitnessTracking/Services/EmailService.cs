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
            var smtpHost = "replace with your smtp server";
            var smtpPort = "replace with your port number";
            var smtpUser = "replace with your smtp user"; 
            var smtpPass = "replace with your smtp password";
            var fromEmailAddress = "repalace with your from email address";
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