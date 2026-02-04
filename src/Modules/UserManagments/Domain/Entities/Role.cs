using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOJ.Modules.UserManagments.Domain.Entities
{
    public class Role : BaseEntity
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public bool IsActive { get; private set; }
        public ICollection<AppUser> Users { get; private set; } = new List<AppUser>();

        // يجب أن يكون constructor عام أو داخلي لكي يعمل مع EF Core
        public Role() { } // Default constructor for EF Core

        public Role(string name, string description = null)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description;
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
        }

        // constructor إضافي لـ seed data
        public Role(int id, string name, string description = null)
        {
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description;
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
        }

        public void Update(string name, string description)
        {
            Name = name;
            Description = description;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Activate()
        {
            IsActive = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }

        // قيم افتراضية للـ Roles
        public static class DefaultRoles
        {
            public const string SuperAdmin = "SuperAdmin";
            public const string Admin = "Admin";
            public const string User = "User";
            public const string Manager = "Manager";
            public const string Editor = "Editor";
            public const string Viewer = "Viewer";
        }
    }
}
