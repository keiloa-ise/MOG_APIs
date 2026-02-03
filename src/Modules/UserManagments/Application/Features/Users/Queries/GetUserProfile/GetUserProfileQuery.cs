using MediatR;
using MOJ.Modules.UserManagments.Application.Features.Users.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOJ.Modules.UserManagments.Application.Features.Users.Queries.GetUserProfile
{
    public class GetUserProfileQuery : IRequest<ApiResponse<UserProfileResponse>>
    {
        public int UserId { get; }

        public GetUserProfileQuery(int userId)
        {
            UserId = userId;
        }
    }

    public class UserProfileResponse
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLogin { get; set; }
        public bool IsActive { get; set; }
    }
}
