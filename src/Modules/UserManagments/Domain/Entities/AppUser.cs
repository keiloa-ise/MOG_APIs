using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOJ.Modules.UserManagments.Domain.Entities
{
    public class AppUser : BaseEntity
    {
        public string Username { get; private set; }
        public string Email { get; private set; }
        public string PasswordHash { get; private set; }
        public string FullName { get; private set; }
        public string PhoneNumber { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime? LastLogin { get; private set; }

        // Foreign Key لـ Role
        public int RoleId { get; private set; }

        // Navigation Property
        public Role Role { get; private set; }
        public ICollection<UserDepartment> UserDepartments { get; private set; } = new List<UserDepartment>();
        private AppUser() { } // For EF Core

        public AppUser(
            string username,
            string email,
            string passwordHash,
            int roleId,
            string fullName = null,
            string phoneNumber = null)
        {
            Username = username ?? throw new ArgumentNullException(nameof(username));
            Email = email ?? throw new ArgumentNullException(nameof(email));
            PasswordHash = passwordHash ?? throw new ArgumentNullException(nameof(passwordHash));
            RoleId = roleId;
            FullName = fullName;
            PhoneNumber = phoneNumber;
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
        }

        public void UpdateLastLogin()
        {
            LastLogin = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Activate()
        {
            IsActive = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateProfile(string fullName, string phoneNumber)
        {
            FullName = fullName;
            PhoneNumber = phoneNumber;
            UpdatedAt = DateTime.UtcNow;
        }

        public void ChangeRole(int roleId)
        {
            RoleId = roleId;
            UpdatedAt = DateTime.UtcNow;
        }

        public bool HasPermission(string permission)
        {
            // يمكن إضافة منطق التحقق من الصلاحيات هنا
            return Role != null && Role.IsActive;
        }
        // إضافة method لتغيير كلمة المرور
        public void ChangePassword(string newPasswordHash)
        {
            PasswordHash = newPasswordHash;
            UpdatedAt = DateTime.UtcNow;
        }
        public bool HasDepartment(int departmentId)
        {
            return UserDepartments?.Any(ud => ud.DepartmentId == departmentId) ?? false;
        }

        public bool IsInDepartment(string departmentCode)
        {
            return UserDepartments?.Any(ud => ud.Department.Code == departmentCode) ?? false;
        }

        public int? GetPrimaryDepartmentId()
        {
            return UserDepartments?.FirstOrDefault(ud => ud.IsPrimary)?.DepartmentId;
        }
    }
}