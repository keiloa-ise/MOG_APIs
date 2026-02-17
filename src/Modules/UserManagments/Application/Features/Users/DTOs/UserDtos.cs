
namespace MOJ.Modules.UserManagments.Application.Features.Users.DTOs
{
    public class UserSignupRequest
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public int RoleId { get; set; } = 3; // Default to "User" role (Id = 3)
        public List<int> DepartmentIds { get; set; } = new();
        public int? PrimaryDepartmentId { get; set; }
    }

    public class UserSignupResponse
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string RoleName { get; set; } 
        public int RoleId { get; set; } 
        public DateTime CreatedAt { get; set; }
        public string Message { get; set; }
        public List<UserDepartmentDto> Departments { get; set; } = new();
    }

    public class CheckAvailabilityRequest
    {
        public string Email { get; set; }
        public string Username { get; set; }
    }

    public class CheckAvailabilityResponse
    {
        public bool Available { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Message { get; set; }
    }
    public class UserProfileResponse
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string RoleName { get; set; } 
        public int RoleId { get; set; } 
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLogin { get; set; }
        public bool IsActive { get; set; }
    }
}
