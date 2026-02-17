namespace MOJ.Modules.UserManagments.Application.Common.Interfaces
{
    public interface IEmailService
    {
        Task SendWelcomeEmail(string email, string fullName);
        Task SendFollowUpEmail(string email);
        Task SendPasswordResetEmail(string email, string resetToken);
        Task SendRoleChangeNotification(string email, string newRole, string changedBy);
        Task SendAccountDeactivatedEmail(string email);
        Task SendEmailAsync(string to, string subject, string body, bool isHtml = true);
    }
}
