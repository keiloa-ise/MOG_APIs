using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOJ.Modules.UserManagments.Application.Features.Users.DTOs
{
    public class UserDepartmentDto
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentNameAr { get; set; }
        public string DepartmentCode { get; set; }
        public bool IsPrimary { get; set; }
        public DateTime AssignedAt { get; set; }
        public string AssignedBy { get; set; }
    }

    public class AssignDepartmentsRequest
    {
        public int UserId { get; set; }
        public List<int> DepartmentIds { get; set; }
        public int? PrimaryDepartmentId { get; set; }
    }

    public class UpdateUserDepartmentsRequest
    {
        public int UserId { get; set; }
        public List<int> AddDepartmentIds { get; set; } = new();
        public List<int> RemoveDepartmentIds { get; set; } = new();
        public int? PrimaryDepartmentId { get; set; }
    }
}
