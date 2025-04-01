using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;

namespace DotCart.Infrastructure.EmailService
{
    public class SmtpEmailService : IEmailService
    {
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _senderEmail;
        private readonly string _senderName;
        private readonly string _senderPassword;

        public SmtpEmailService(IConfiguration configuration)
        {
            var emailSettings = configuration.GetSection("EmailSettings");
            _smtpServer = emailSettings["SmtpServer"] ?? throw new ArgumentNullException("SMTP Server is missing");
            _smtpPort = int.Parse(emailSettings["SmtpPort"] ?? "587");
            _senderEmail = emailSettings["SenderEmail"] ?? throw new ArgumentNullException("Sender Email is missing");
            _senderName = emailSettings["SenderName"] ?? "DotCart";
            _senderPassword = emailSettings["SenderPassword"] ?? throw new ArgumentNullException("Sender Password is missing");
        }

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(_senderName, _senderEmail));
            emailMessage.To.Add(new MailboxAddress("", toEmail));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart("html") { Text = message };

            using var client = new SmtpClient();
            try
            {
                await client.ConnectAsync(_smtpServer, _smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_senderEmail, _senderPassword);
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email sending failed: {ex.Message}");
                return false;
            }
        }
    }
}