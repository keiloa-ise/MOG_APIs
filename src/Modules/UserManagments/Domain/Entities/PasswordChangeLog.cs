
namespace MOJ.Modules.UserManagments.Domain.Entities
{
    public class PasswordChangeLog : BaseEntity
    {
        public int UserId { get; private set; }
        public AppUser User { get; private set; }

        public int ChangedByUserId { get; private set; }
        public AppUser ChangedByUser { get; private set; }

        public string ChangeType { get; private set; }
        public string IpAddress { get; private set; }
        public string UserAgent { get; private set; }
        public string PasswordHash { get; private set; }

        // Constructor لـ EF Core
        public PasswordChangeLog() { }

        // Constructor كامل
        public PasswordChangeLog(
            int userId,
            int changedByUserId,
            string changeType,
            string ipAddress = null,
            string userAgent = null,
            string passwordHash = null)
        {
            UserId = userId;
            ChangedByUserId = changedByUserId;
            ChangeType = changeType;
            IpAddress = ipAddress;
            UserAgent = userAgent;
            PasswordHash = passwordHash;
            CreatedAt = DateTime.UtcNow;
        }

        // Factory method
        public static PasswordChangeLog Create(
            int userId,
            int changedByUserId,
            string changeType,
            string ipAddress = null,
            string userAgent = null,
            string passwordHash = null)
        {
            return new PasswordChangeLog
            {
                UserId = userId,
                ChangedByUserId = changedByUserId,
                ChangeType = changeType,
                IpAddress = ipAddress,
                UserAgent = userAgent,
                PasswordHash = passwordHash,
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}
