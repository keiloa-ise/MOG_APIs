using MediatR;
using MOJ.Modules.UserManagments.Application.Features.Users.DTOs;
using MOJ.Shared.Application.DTOs;

namespace MOJ.Modules.UserManagments.Application.Features.Users.Commands.AssignDepartments
{
    public class AssignDepartmentsCommand : IRequest<ApiResponse<List<UserDepartmentDto>>>
    {
        public AssignDepartmentsRequest Request { get; }
        public int CurrentUserId { get; }

        public AssignDepartmentsCommand(AssignDepartmentsRequest request, int currentUserId)
        {
            Request = request;
            CurrentUserId = currentUserId;
        }
    }
}
