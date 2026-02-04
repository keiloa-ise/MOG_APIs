using MediatR;
using MOJ.Modules.UserManagments.Application.Features.Users.DTOs;
using MOJ.Shared.Application.DTOs;
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
}
