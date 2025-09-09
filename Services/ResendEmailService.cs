using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Craftmatrix.org.API.Services
{
    public class ResendEmailService : IEmailService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ResendEmailService> _logger;
        private readonly string _apiKey;
        private readonly string _fromEmail;

        public ResendEmailService(HttpClient httpClient, ILogger<ResendEmailService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _apiKey = Environment.GetEnvironmentVariable("RESEND_API_KEY") ?? throw new InvalidOperationException("RESEND_API_KEY environment variable is not set");
            _fromEmail = "noreply@siglat.app"; // Change this to your verified domain
            
            _httpClient.BaseAddress = new Uri("https://api.resend.com/");
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
        }

        public async Task<bool> SendOtpEmailAsync(string toEmail, string otpCode, string recipientName = "User")
        {
            try
            {
                var emailContent = new
                {
                    from = _fromEmail,
                    to = new[] { toEmail },
                    subject = "Your Admin Registration OTP Code",
                    html = GenerateOtpEmailHtml(otpCode, recipientName)
                };

                var json = JsonSerializer.Serialize(emailContent);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("emails", content);
                
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"OTP email sent successfully to {toEmail}");
                    return true;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Failed to send OTP email to {toEmail}. Status: {response.StatusCode}, Error: {errorContent}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception occurred while sending OTP email to {toEmail}");
                return false;
            }
        }

        public async Task<bool> SendWelcomeEmailAsync(string toEmail, string recipientName)
        {
            try
            {
                var emailContent = new
                {
                    from = _fromEmail,
                    to = new[] { toEmail },
                    subject = "Welcome to SIGLAT",
                    html = GenerateWelcomeEmailHtml(recipientName)
                };

                var json = JsonSerializer.Serialize(emailContent);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("emails", content);
                
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"Welcome email sent successfully to {toEmail}");
                    return true;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Failed to send welcome email to {toEmail}. Status: {response.StatusCode}, Error: {errorContent}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception occurred while sending welcome email to {toEmail}");
                return false;
            }
        }

        public async Task<bool> SendPasswordResetEmailAsync(string toEmail, string resetLink, string recipientName)
        {
            try
            {
                var emailContent = new
                {
                    from = _fromEmail,
                    to = new[] { toEmail },
                    subject = "Password Reset Request",
                    html = GeneratePasswordResetEmailHtml(resetLink, recipientName)
                };

                var json = JsonSerializer.Serialize(emailContent);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("emails", content);
                
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"Password reset email sent successfully to {toEmail}");
                    return true;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Failed to send password reset email to {toEmail}. Status: {response.StatusCode}, Error: {errorContent}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception occurred while sending password reset email to {toEmail}");
                return false;
            }
        }

        private string GenerateOtpEmailHtml(string otpCode, string recipientName)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Your OTP Code</title>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #007bff; color: white; padding: 20px; text-align: center; }}
        .content {{ padding: 30px; background-color: #f9f9f9; }}
        .otp-code {{ font-size: 32px; font-weight: bold; color: #007bff; text-align: center; padding: 20px; background-color: white; border: 2px dashed #007bff; margin: 20px 0; }}
        .footer {{ padding: 20px; text-align: center; color: #666; font-size: 12px; }}
        .warning {{ background-color: #fff3cd; border: 1px solid #ffeaa7; padding: 15px; margin: 15px 0; border-radius: 4px; }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>SIGLAT Admin Registration</h1>
        </div>
        <div class=""content"">
            <h2>Hello {recipientName},</h2>
            <p>You requested to create an admin account. Use the following OTP code to complete your registration:</p>
            <div class=""otp-code"">{otpCode}</div>
            <div class=""warning"">
                <strong>Important:</strong>
                <ul>
                    <li>This code expires in 10 minutes</li>
                    <li>Do not share this code with anyone</li>
                    <li>If you didn't request this, please ignore this email</li>
                </ul>
            </div>
            <p>If you have any questions, please contact our support team.</p>
        </div>
        <div class=""footer"">
            <p>&copy; 2024 SIGLAT. All rights reserved.</p>
            <p>This is an automated email, please do not reply.</p>
        </div>
    </div>
</body>
</html>";
        }

        private string GenerateWelcomeEmailHtml(string recipientName)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Welcome to SIGLAT</title>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #28a745; color: white; padding: 20px; text-align: center; }}
        .content {{ padding: 30px; background-color: #f9f9f9; }}
        .footer {{ padding: 20px; text-align: center; color: #666; font-size: 12px; }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>Welcome to SIGLAT!</h1>
        </div>
        <div class=""content"">
            <h2>Hello {recipientName},</h2>
            <p>Welcome to SIGLAT! Your account has been successfully created and is ready to use.</p>
            <p>You can now log in to your account and start using our services.</p>
            <p>If you have any questions or need assistance, please don't hesitate to contact our support team.</p>
        </div>
        <div class=""footer"">
            <p>&copy; 2024 SIGLAT. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";
        }

        private string GeneratePasswordResetEmailHtml(string resetLink, string recipientName)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Password Reset</title>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #dc3545; color: white; padding: 20px; text-align: center; }}
        .content {{ padding: 30px; background-color: #f9f9f9; }}
        .button {{ display: inline-block; padding: 12px 24px; background-color: #dc3545; color: white; text-decoration: none; border-radius: 4px; margin: 15px 0; }}
        .footer {{ padding: 20px; text-align: center; color: #666; font-size: 12px; }}
        .warning {{ background-color: #f8d7da; border: 1px solid #f5c6cb; padding: 15px; margin: 15px 0; border-radius: 4px; }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>Password Reset Request</h1>
        </div>
        <div class=""content"">
            <h2>Hello {recipientName},</h2>
            <p>You requested a password reset for your SIGLAT account. Click the button below to reset your password:</p>
            <p style=""text-align: center;"">
                <a href=""{resetLink}"" class=""button"">Reset Password</a>
            </p>
            <div class=""warning"">
                <strong>Security Notice:</strong>
                <ul>
                    <li>This link expires in 1 hour</li>
                    <li>If you didn't request this reset, please ignore this email</li>
                    <li>Your password won't change until you click the link above</li>
                </ul>
            </div>
        </div>
        <div class=""footer"">
            <p>&copy; 2024 SIGLAT. All rights reserved.</p>
            <p>This is an automated email, please do not reply.</p>
        </div>
    </div>
</body>
</html>";
        }
    }
}