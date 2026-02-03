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
        public string Role { get; private set; }
        public DateTime? LastLogin { get; private set; }

        private AppUser() { } // For EF Core

        public AppUser(
            string username,
            string email,
            string passwordHash,
            string fullName = null,
            string phoneNumber = null)
        {
            Username = username ?? throw new ArgumentNullException(nameof(username));
            Email = email ?? throw new ArgumentNullException(nameof(email));
            PasswordHash = passwordHash ?? throw new ArgumentNullException(nameof(passwordHash));
            FullName = fullName;
            PhoneNumber = phoneNumber;
            IsActive = true;
            Role = "User";
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
    }
}