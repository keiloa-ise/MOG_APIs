
namespace MOJ.Modules.UserManagments.Application.Features.Departments.DTOs
{
    public class DepartmentDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string NameAr { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public int? ParentDepartmentId { get; set; }
        public string ParentDepartmentName { get; set; }
        public DateTime CreatedAt { get; set; }
        public int UsersCount { get; set; }
        public List<DepartmentDto> ChildDepartments { get; set; }
    }

    public class CreateDepartmentRequest
    {
        public string Name { get; set; }
        public string NameAr { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public int? ParentDepartmentId { get; set; }
    }

    public class UpdateDepartmentRequest
    {
        public string Name { get; set; }
        public string NameAr { get; set; }
        public string Description { get; set; }
        public int? ParentDepartmentId { get; set; }
        public bool? IsActive { get; set; }
    }
}
