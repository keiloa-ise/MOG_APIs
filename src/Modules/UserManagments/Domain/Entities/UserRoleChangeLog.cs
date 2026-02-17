namespace MOJ.Modules.UserManagments.Domain.Entities
{
    public class UserRoleChangeLog : BaseEntity
    {
        public int UserId { get; private set; }
        public AppUser User { get; private set; }

        public int PreviousRoleId { get; private set; }
        public Role PreviousRole { get; private set; }

        public int NewRoleId { get; private set; }
        public Role NewRole { get; private set; }

        public int ChangedByUserId { get; private set; }
        public AppUser ChangedByUser { get; private set; }

        public string Reason { get; private set; }

        // Constructor لـ EF Core
        public UserRoleChangeLog() { }

        // Constructor كامل
        public UserRoleChangeLog(
            int userId,
            int previousRoleId,
            int newRoleId,
            int changedByUserId,
            string reason = null)
        {
            UserId = userId;
            PreviousRoleId = previousRoleId;
            NewRoleId = newRoleId;
            ChangedByUserId = changedByUserId;
            Reason = reason;
            CreatedAt = DateTime.UtcNow;
        }

        // Factory method بديلة
        public static UserRoleChangeLog Create(
            int userId,
            int previousRoleId,
            int newRoleId,
            int changedByUserId,
            string reason = null)
        {
            return new UserRoleChangeLog
            {
                UserId = userId,
                PreviousRoleId = previousRoleId,
                NewRoleId = newRoleId,
                ChangedByUserId = changedByUserId,
                Reason = reason,
                CreatedAt = DateTime.UtcNow
            };
        }

        // Methods لتعيين القيم (إذا أردت التعيين المنفصل)
        public void SetUserId(int userId) => UserId = userId;
        public void SetPreviousRoleId(int previousRoleId) => PreviousRoleId = previousRoleId;
        public void SetNewRoleId(int newRoleId) => NewRoleId = newRoleId;
        public void SetChangedByUserId(int changedByUserId) => ChangedByUserId = changedByUserId;
        public void SetReason(string reason) => Reason = reason;
    }
}
