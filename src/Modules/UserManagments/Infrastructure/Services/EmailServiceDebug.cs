using Microsoft.Extensions.Logging;
using MOJ.Modules.UserManagments.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOJ.Modules.UserManagments.Infrastructure.Services
{
    // للاستخدام في بيئة التطوير فقط - يكتب الإيميلات في Console بدلاً من إرسالها
    public class EmailServiceDebug : IEmailService
    {
        private readonly ILogger<EmailServiceDebug> _logger;

        public EmailServiceDebug(ILogger<EmailServiceDebug> logger)
        {
            _logger = logger;
        }

        public Task SendWelcomeEmail(string email, string fullName)
        {
            _logger.LogInformation("[DEV] Sending welcome email to {Email} for {FullName}", email, fullName);
            return Task.CompletedTask;
        }

        public Task SendFollowUpEmail(string email)
        {
            _logger.LogInformation("[DEV] Sending follow-up email to {Email}", email);
            return Task.CompletedTask;
        }

        public Task SendPasswordResetEmail(string email, string resetToken)
        {
            _logger.LogInformation("[DEV] Sending password reset email to {Email} with token: {Token}", email, resetToken);
            return Task.CompletedTask;
        }

        public Task SendRoleChangeNotification(string email, string newRole, string changedBy)
        {
            _logger.LogInformation("[DEV] Sending role change notification to {Email}: New Role={NewRole}, Changed By={ChangedBy}",
                email, newRole, changedBy);
            return Task.CompletedTask;
        }

        public Task SendAccountDeactivatedEmail(string email)
        {
            _logger.LogInformation("[DEV] Sending account deactivation email to {Email}", email);
            return Task.CompletedTask;
        }

        public Task SendEmailAsync(string to, string subject, string body, bool isHtml = true)
        {
            _logger.LogInformation("[DEV] Email to: {To}, Subject: {Subject}", to, subject);
            _logger.LogDebug("Body: {Body}", body);
            return Task.CompletedTask;
        }
    }
}
