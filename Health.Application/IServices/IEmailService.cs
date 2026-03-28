using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Health.Application.IServices
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string message);

        // For sending the specific password reset email
        Task SendPasswordResetEmailAsync(string email, string userId, string token);
    }
}
