using Microsoft.AspNetCore.WebUtilities;
using System.Net;
using System.Net.Mail;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Iemailsender.Services;
namespace Emailsender.Services
{
    public class CustomEmailSender : ICustomEmailSender
    {
        private readonly SmtpClient _smtpClient;
        private readonly IConfiguration _configuration;

        public CustomEmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
            _smtpClient = new SmtpClient
            {
                Host = _configuration["Email:Host"],
                Port = int.Parse(_configuration["Email:Port"]),
                EnableSsl = true,
                Credentials = new NetworkCredential(_configuration["Email:Username"], _configuration["Email:Password"])
            };
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            try
            {
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_configuration["Email:Username"]),
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = true,
                };

                mailMessage.To.Add(email);

                await _smtpClient.SendMailAsync(mailMessage);
                Console.WriteLine("Email sent successfully to: " + email);

                Console.WriteLine("Email sent successfully.");
            }
            catch (SmtpException smtpEx)
            {
                Console.WriteLine($"SMTP error: {smtpEx.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General error: {ex.Message}");
            }
        }


    }
}
