using MediatR;
using MOJ.Modules.UserManagments.Application.Features.Users.DTOs;
using MOJ.Shared.Application.DTOs;

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
