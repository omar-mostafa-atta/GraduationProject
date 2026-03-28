using Health.Application.IServices;
using Health.Application.Models;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using MailKit.Net.Smtp;

namespace Health.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        // Dependency Injection for EmailSettings
        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
            emailMessage.To.Add(new MailboxAddress("", toEmail));
            emailMessage.Subject = subject;

            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message
            };

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                try
                {
                    await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, MailKit.Security.SecureSocketOptions.StartTls);

                    await client.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password);

                    await client.SendAsync(emailMessage);
                }
                catch
                {
                    
                    throw;
                }
                finally
                {
                    await client.DisconnectAsync(true);
                }
            }
        }

   
        public async Task SendPasswordResetEmailAsync(string email, string userId, string token)
        {
          
            var encodedToken = System.Web.HttpUtility.UrlEncode(token);

            var resetUrl = $"the front end url/reset-password?userId={userId}&token={encodedToken}";

            var subject = "Reset Your Password";
            var message = $@"
                <h1>Password Reset Request</h1>
                <p>You requested a password reset for your account.</p>
                <p>Please click the following link to reset your password:</p>
                <a href='{resetUrl}'>Reset Password</a>
                <p>If you did not request this, please ignore this email.</p>";

            await SendEmailAsync(email, subject, message);
        }
    }
}
