using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MOJ.Modules.UserManagments.Application.Common.Interfaces;

namespace MOJ.Modules.UserManagments.Infrastructure.Services
{
    public class CleanupService : ICleanupService
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<CleanupService> _logger;
        private readonly IDateTime _dateTime;

        public CleanupService(
            IApplicationDbContext context,
            ILogger<CleanupService> logger,
            IDateTime dateTime)
        {
            _context = context;
            _logger = logger;
            _dateTime = dateTime;
        }

        public async Task CleanExpiredSessions()
        {
            try
            {
                _logger.LogInformation("Starting cleanup of expired sessions at {Time}", _dateTime.UtcNow);

                // تنظيف سجلات تغيير كلمة المرور القديمة
                var oldPasswordChanges = await _context.PasswordChangeLogs
                    .Where(p => p.CreatedAt < _dateTime.UtcNow.AddMonths(-6))
                    .ToListAsync();

                if (oldPasswordChanges.Any())
                {
                    _context.PasswordChangeLogs.RemoveRange(oldPasswordChanges);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Removed {Count} old password change logs", oldPasswordChanges.Count);
                }

                // تحديث حالة المستخدمين غير النشطين
                var inactiveUsers = await _context.AppUsers
                    .Where(u => u.LastLogin != null &&
                                u.LastLogin < _dateTime.UtcNow.AddDays(-90) &&
                                u.IsActive)
                    .ToListAsync();

                foreach (var user in inactiveUsers)
                {
                    user.Deactivate();
                }

                if (inactiveUsers.Any())
                {
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Deactivated {Count} inactive users", inactiveUsers.Count);
                }

                _logger.LogInformation("Completed cleanup of expired sessions at {Time}", _dateTime.UtcNow);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during cleanup of expired sessions");
            }
        }

        public async Task CleanOldPasswordHistories(int keepLast = 5)
        {
            try
            {
                // الحصول على جميع المستخدمين
                var users = await _context.AppUsers
                    .Select(u => u.Id)
                    .ToListAsync();

                foreach (var userId in users)
                {
                    // الحصول على تاريخ كلمات المرور لكل مستخدم
                    var passwordHistories = await _context.PasswordHistories
                        .Where(p => p.UserId == userId)
                        .OrderByDescending(p => p.CreatedAt)
                        .ToListAsync();

                    if (passwordHistories.Count > keepLast)
                    {
                        var toRemove = passwordHistories.Skip(keepLast).ToList();
                        _context.PasswordHistories.RemoveRange(toRemove);
                    }
                }

                await _context.SaveChangesAsync();
                _logger.LogInformation("Cleaned old password histories");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cleaning password histories");
            }
        }

        public async Task CleanInactiveUsers(int daysInactive = 90)
        {
            try
            {
                var cutoffDate = _dateTime.UtcNow.AddDays(-daysInactive);

                var inactiveUsers = await _context.AppUsers
                    .Where(u => u.LastLogin < cutoffDate && u.IsActive)
                    .ToListAsync();

                foreach (var user in inactiveUsers)
                {
                    user.Deactivate();
                }

                await _context.SaveChangesAsync();
                _logger.LogInformation("Deactivated {Count} users inactive for {Days} days",
                    inactiveUsers.Count, daysInactive);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cleaning inactive users");
            }
        }

        public async Task CleanOldAuditLogs(int daysToKeep = 30)
        {
            try
            {
                var cutoffDate = _dateTime.UtcNow.AddDays(-daysToKeep);

                // تنظيف UserRoleChangeLogs القديمة
                var oldRoleChanges = await _context.UserRoleChangeLogs
                    .Where(l => l.CreatedAt < cutoffDate)
                    .ToListAsync();

                if (oldRoleChanges.Any())
                {
                    _context.UserRoleChangeLogs.RemoveRange(oldRoleChanges);
                }

                // تنظيف PasswordChangeLogs القديمة
                var oldPasswordChanges = await _context.PasswordChangeLogs
                    .Where(l => l.CreatedAt < cutoffDate)
                    .ToListAsync();

                if (oldPasswordChanges.Any())
                {
                    _context.PasswordChangeLogs.RemoveRange(oldPasswordChanges);
                }

                await _context.SaveChangesAsync();
                _logger.LogInformation("Cleaned audit logs older than {Days} days", daysToKeep);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cleaning audit logs");
            }
        }
    }
}
