using System.Threading.Tasks;

namespace Craftmatrix.org.API.Services
{
    public interface IEmailService
    {
        Task<bool> SendOtpEmailAsync(string toEmail, string otpCode, string recipientName = "User");
        Task<bool> SendWelcomeEmailAsync(string toEmail, string recipientName);
        Task<bool> SendPasswordResetEmailAsync(string toEmail, string resetLink, string recipientName);
    }
}