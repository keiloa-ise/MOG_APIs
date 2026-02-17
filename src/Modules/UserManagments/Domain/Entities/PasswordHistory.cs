
namespace MOJ.Modules.UserManagments.Domain.Entities
{
    public class PasswordHistory : BaseEntity
    {
        public int UserId { get; private set; }
        public AppUser User { get; private set; }

        public string PasswordHash { get; private set; }

        // Constructor لـ EF Core
        public PasswordHistory() { }

        // Constructor كامل
        public PasswordHistory(int userId, string passwordHash)
        {
            UserId = userId;
            PasswordHash = passwordHash;
            CreatedAt = DateTime.UtcNow;
        }

        // Factory method
        public static PasswordHistory Create(int userId, string passwordHash)
        {
            return new PasswordHistory
            {
                UserId = userId,
                PasswordHash = passwordHash,
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}
