
namespace MOJ.Modules.UserManagments.Domain.Entities
{
    public class Department : BaseEntity
    {
        public string Name { get; private set; }
        public string NameAr { get; private set; } 
        public string Code { get; private set; }
        public string Description { get; private set; }
        public bool IsActive { get; private set; }
        public int? ParentDepartmentId { get; private set; }

        // Navigation Properties
        public Department ParentDepartment { get; private set; }
        public ICollection<Department> ChildDepartments { get; private set; } = new List<Department>();
        public ICollection<UserDepartment> UserDepartments { get; private set; } = new List<UserDepartment>();

        private Department() { }

        public Department(string name, string nameAr, string code, string description = null, int? parentDepartmentId = null)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            NameAr = nameAr ?? throw new ArgumentNullException(nameof(nameAr));
            Code = code ?? throw new ArgumentNullException(nameof(code));
            Description = description;
            ParentDepartmentId = parentDepartmentId;
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
        }

        public void Update(string name, string nameAr, string description, int? parentDepartmentId = null)
        {
            Name = name;
            NameAr = nameAr;
            Description = description;
            ParentDepartmentId = parentDepartmentId;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Activate() => IsActive = true;
        public void Deactivate() => IsActive = false;
    }
}
