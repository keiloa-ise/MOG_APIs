using MediatR;
using MOJ.Modules.UserManagments.Application.Features.Users.DTOs;
using MOJ.Shared.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOJ.Modules.UserManagments.Application.Features.Users.Queries.GetUserDepartments
{
    public class GetUserDepartmentsQuery : IRequest<ApiResponse<List<UserDepartmentDto>>>
    {
        public int UserId { get; }

        public GetUserDepartmentsQuery(int userId)
        {
            UserId = userId;
        }
    }
}
