using System.Threading.Tasks;

namespace Iemailsender.Services
{
    public interface ICustomEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
