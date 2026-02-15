using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOJ.Modules.UserManagments.Domain.Entities
{
    public class UserDepartment : BaseEntity
    {
        public int UserId { get; private set; }
        public AppUser User { get; private set; }

        public int DepartmentId { get; private set; }
        public Department Department { get; private set; }

        public bool IsPrimary { get; private set; } // القسم الأساسي
        public DateTime AssignedAt { get; private set; }
        public int AssignedByUserId { get; private set; }
        public AppUser AssignedByUser { get; private set; }

        private UserDepartment() { }

        public UserDepartment(int userId, int departmentId, int assignedByUserId, bool isPrimary = false)
        {
            UserId = userId;
            DepartmentId = departmentId;
            AssignedByUserId = assignedByUserId;
            IsPrimary = isPrimary;
            AssignedAt = DateTime.UtcNow;
            CreatedAt = DateTime.UtcNow;
        }

        public void SetAsPrimary() => IsPrimary = true;
        public void RemoveAsPrimary() => IsPrimary = false;
    }
}
