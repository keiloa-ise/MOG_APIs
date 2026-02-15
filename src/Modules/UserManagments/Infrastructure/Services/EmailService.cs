using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using MOJ.Modules.UserManagments.Application.Common.Models;
using MailKit.Net.Smtp;
using MOJ.Modules.UserManagments.Application.Common.Interfaces;

namespace MOJ.Modules.UserManagments.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
        }

        public async Task SendWelcomeEmail(string email, string fullName)
        {
            var subject = "Welcome to MOJ Platform!";
            var body = $@"
            <!DOCTYPE html>
            <html>
            <head>
                <style>
                    body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                    .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                    .header {{ background-color: #4CAF50; color: white; padding: 20px; text-align: center; }}
                    .content {{ padding: 20px; }}
                    .footer {{ background-color: #f4f4f4; padding: 10px; text-align: center; font-size: 12px; }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'>
                        <h1>Welcome to MOJ Platform!</h1>
                    </div>
                    <div class='content'>
                        <p>Dear {fullName},</p>
                        <p>Thank you for registering with the Ministry of Justice platform. We're excited to have you on board!</p>
                        <p>Your account has been successfully created. You can now:</p>
                        <ul>
                            <li>Access your dashboard</li>
                            <li>Manage your profile</li>
                            <li>Explore available services</li>
                        </ul>
                        <p>If you have any questions, please don't hesitate to contact our support team.</p>
                        <p>Best regards,<br/>MOJ Team</p>
                    </div>
                    <div class='footer'>
                        <p>&copy; 2024 Ministry of Justice. All rights reserved.</p>
                    </div>
                </div>
            </body>
            </html>";

            await SendEmailAsync(email, subject, body);
        }

        public async Task SendFollowUpEmail(string email)
        {
            var subject = "Getting Started with MOJ Platform";
            var body = $@"
            <!DOCTYPE html>
            <html>
            <head>
                <style>
                    body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                    .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                    .header {{ background-color: #2196F3; color: white; padding: 20px; text-align: center; }}
                    .content {{ padding: 20px; }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'>
                        <h2>Getting Started Guide</h2>
                    </div>
                    <div class='content'>
                        <p>Hello,</p>
                        <p>It's been a few days since you joined MOJ Platform. We wanted to check in and help you get the most out of your account.</p>
                        <h3>Quick Tips:</h3>
                        <ul>
                            <li>Complete your profile information</li>
                            <li>Set up two-factor authentication</li>
                            <li>Explore our documentation</li>
                        </ul>
                        <p>Need help? Our support team is always here for you.</p>
                        <p>Best regards,<br/>MOJ Team</p>
                    </div>
                </div>
            </body>
            </html>";

            await SendEmailAsync(email, subject, body);
        }

        public async Task SendPasswordResetEmail(string email, string resetToken)
        {
            var subject = "Password Reset Request";
            var resetLink = $"https://yourdomain.com/reset-password?token={resetToken}";

            var body = $@"
            <!DOCTYPE html>
            <html>
            <head>
                <style>
                    body {{ font-family: Arial, sans-serif; line-height: 1.6; }}
                    .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                    .button {{
                        background-color: #4CAF50;
                        border: none;
                        color: white;
                        padding: 15px 32px;
                        text-align: center;
                        text-decoration: none;
                        display: inline-block;
                        font-size: 16px;
                        margin: 4px 2px;
                        cursor: pointer;
                        border-radius: 4px;
                    }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <h2>Password Reset Request</h2>
                    <p>You recently requested to reset your password. Click the button below to proceed:</p>
                    <a href='{resetLink}' class='button'>Reset Password</a>
                    <p>If you didn't request this, please ignore this email or contact support.</p>
                    <p>This link will expire in 1 hour.</p>
                    <p>Best regards,<br/>MOJ Team</p>
                </div>
            </body>
            </html>";

            await SendEmailAsync(email, subject, body);
        }

        public async Task SendRoleChangeNotification(string email, string newRole, string changedBy)
        {
            var subject = "Your Role Has Been Updated";
            var body = $@"
            <!DOCTYPE html>
            <html>
            <head>
                <style>
                    body {{ font-family: Arial, sans-serif; line-height: 1.6; }}
                    .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <h2>Role Update Notification</h2>
                    <p>Your account role has been updated.</p>
                    <p><strong>New Role:</strong> {newRole}</p>
                    <p><strong>Changed By:</strong> {changedBy}</p>
                    <p><strong>Date:</strong> {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC</p>
                    <p>If you have any questions about this change, please contact your administrator.</p>
                    <p>Best regards,<br/>MOJ Team</p>
                </div>
            </body>
            </html>";

            await SendEmailAsync(email, subject, body);
        }

        public async Task SendAccountDeactivatedEmail(string email)
        {
            var subject = "Account Deactivation Notice";
            var body = $@"
            <!DOCTYPE html>
            <html>
            <head>
                <style>
                    body {{ font-family: Arial, sans-serif; line-height: 1.6; }}
                    .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <h2>Account Deactivation</h2>
                    <p>Your account has been deactivated due to inactivity.</p>
                    <p>If you wish to reactivate your account, please contact our support team.</p>
                    <p>Best regards,<br/>MOJ Team</p>
                </div>
            </body>
            </html>";

            await SendEmailAsync(email, subject, body);
        }

        public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = true)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_emailSettings.FromName, _emailSettings.FromEmail));
                message.To.Add(new MailboxAddress("", to));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder();
                if (isHtml)
                    bodyBuilder.HtmlBody = body;
                else
                    bodyBuilder.TextBody = body;

                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();

                // For development only - remove in production
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                await client.ConnectAsync(
                    _emailSettings.SmtpServer,
                    _emailSettings.SmtpPort,
                    _emailSettings.EnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None
                );

                if (!string.IsNullOrEmpty(_emailSettings.SmtpUsername))
                {
                    await client.AuthenticateAsync(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword);
                }

                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation("Email sent successfully to {To}", to);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {To}. Subject: {Subject}", to, subject);
                throw; // Re-throw for Hangfire to handle retries
            }
        }
    }
}
