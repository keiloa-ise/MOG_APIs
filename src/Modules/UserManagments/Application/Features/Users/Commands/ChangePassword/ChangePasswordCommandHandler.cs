using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MOJ.Modules.UserManagments.Application.Common.Interfaces;
using MOJ.Modules.UserManagments.Application.Common.Services;
using MOJ.Modules.UserManagments.Domain.Entities;
using MOJ.Shared.Application.DTOs;

namespace MOJ.Modules.UserManagments.Application.Features.Users.Commands.ChangePassword
{
    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, ApiResponse<ChangePasswordResponse>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<ChangePasswordCommandHandler> _logger;
        private readonly IDateTime _dateTime;
        private readonly IPasswordPolicyService _passwordPolicyService;

        public ChangePasswordCommandHandler(
            IApplicationDbContext context,
            ILogger<ChangePasswordCommandHandler> logger,
            IDateTime dateTime,
            IPasswordPolicyService passwordPolicyService)
        {
            _context = context;
            _logger = logger;
            _dateTime = dateTime;
            _passwordPolicyService = passwordPolicyService;
        }

        public async Task<ApiResponse<ChangePasswordResponse>> Handle(
            ChangePasswordCommand command,
            CancellationToken cancellationToken)
        {
            try
            {
                // 1. Checking the user's presence
                var user = await _context.AppUsers
                    .FirstOrDefaultAsync(u => u.Id == command.CurrentUserId, cancellationToken);

                if (user == null)
                {
                    return ApiResponse<ChangePasswordResponse>.Error(
                        "User not found",
                        new List<string> { "User does not exist" });
                }

                if (!user.IsActive)
                {
                    return ApiResponse<ChangePasswordResponse>.Error(
                        "Account is inactive",
                        new List<string> { "Cannot change password for inactive account" });
                }

                // 2. Check your current password
                var isCurrentPasswordValid = BCrypt.Net.BCrypt.Verify(
                    command.Request.CurrentPassword,
                    user.PasswordHash);

                if (!isCurrentPasswordValid)
                {
                    _logger.LogWarning("Invalid current password for UserId={UserId}", user.Id);
                    return ApiResponse<ChangePasswordResponse>.Error(
                        "Invalid current password",
                        new List<string> { "The current password you entered is incorrect" });
                }

                // 3. Check the strength of the new password
                var passwordErrors = _passwordPolicyService.ValidatePasswordStrength(
                    command.Request.NewPassword);

                if (passwordErrors.Any())
                {
                    return ApiResponse<ChangePasswordResponse>.Error(
                        "Weak password",
                        passwordErrors);
                }

                //4. Verify that the same password was not used previously.
                var isSameAsOldPassword = BCrypt.Net.BCrypt.Verify(
                    command.Request.NewPassword,
                    user.PasswordHash);

                if (isSameAsOldPassword)
                {
                    return ApiResponse<ChangePasswordResponse>.Error(
                        "Password reuse not allowed",
                        new List<string> { "New password cannot be the same as current password" });
                }

                // 5. Check the history of previous passwords
                var passwordHistory = await GetPasswordHistory(user.Id, cancellationToken);
                foreach (var oldHash in passwordHistory)
                {
                    if (BCrypt.Net.BCrypt.Verify(command.Request.NewPassword, oldHash))
                    {
                        return ApiResponse<ChangePasswordResponse>.Error(
                            "Password previously used",
                            new List<string> { "You cannot reuse a previously used password" });
                    }
                }

                //6. Encrypt the new password
                var newPasswordHash = BCrypt.Net.BCrypt.HashPassword(
                    command.Request.NewPassword,
                    workFactor: 12);

                // 7. Save the old hash before updating
                var oldPasswordHash = user.PasswordHash;

                // 8. Update the password in the database
                user.ChangePassword(newPasswordHash); // استخدم method ChangePassword

                _context.AppUsers.Update(user);

                //9. Save password change history (PasswordHistory)
                var passwordHistoryRecord = PasswordHistory.Create(user.Id, newPasswordHash);
                await _context.PasswordHistories.AddAsync(passwordHistoryRecord, cancellationToken);

                // 10. Create an audit log.
                var passwordChangeLog = PasswordChangeLog.Create(
                    userId: user.Id,
                    changedByUserId: command.CurrentUserId,
                    changeType: "ManualChange",
                    ipAddress: "",//GetClientIpAddress(),
                    userAgent: "",// GetUserAgent(),
                    passwordHash: oldPasswordHash // حفظ الـ hash القديم للأرشفة
                );

                await _context.PasswordChangeLogs.AddAsync(passwordChangeLog, cancellationToken);

                // 11. Save changes
                await _context.SaveChangesAsync(cancellationToken);

                // 
                var response = new ChangePasswordResponse
                {
                    UserId = user.Id.ToString(),
                    Username = user.Username,
                    Email = user.Email,
                    ChangedAt = _dateTime.UtcNow,
                    Message = "Password changed successfully"
                };

                _logger.LogInformation("Password changed successfully for UserId={UserId}", user.Id);

                return ApiResponse<ChangePasswordResponse>.Success(response, "Password changed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password for UserId={UserId}", command.CurrentUserId);
                return ApiResponse<ChangePasswordResponse>.Error(
                    "An error occurred while changing password",
                    new List<string> { ex.Message });
            }
        }

        private async Task<List<string>> GetPasswordHistory(int userId, CancellationToken cancellationToken)
        {
            try
            {
                // Get the last 5 passwords from the PasswordHistories table
                var recentPasswords = await _context.PasswordHistories
                    .Where(p => p.UserId == userId)
                    .OrderByDescending(p => p.CreatedAt)
                    .Take(5)
                    .Select(p => p.PasswordHash)
                    .ToListAsync(cancellationToken);

                return recentPasswords;
            }
            catch
            {                
                return new List<string>();
            }
        }

        //private string GetClientIpAddress()
        //{
        //    // يمكن تمرير HttpContext إلى الـ Handler أو استخدام IHttpContextAccessor
        //    // هذا مثال بسيط
        //    return HttpContext.Connection?.RemoteIpAddress?.ToString() ?? "N/A";
        //}

        //private string GetUserAgent()
        //{
        //    // يمكن تمرير HttpContext إلى الـ Handler
        //    return HttpContext.Request.Headers["User-Agent"].ToString() ?? "N/A";
        //}
    }
}
