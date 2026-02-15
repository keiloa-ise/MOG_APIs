using MediatR;
using MOJ.Modules.UserManagments.Application.Features.Users.DTOs;
using MOJ.Shared.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOJ.Modules.UserManagments.Application.Features.Users.Commands.UpdateUserDepartments
{
    public class UpdateUserDepartmentsCommand : IRequest<ApiResponse<List<UserDepartmentDto>>>
    {
        public UpdateUserDepartmentsRequest Request { get; }
        public int CurrentUserId { get; }

        public UpdateUserDepartmentsCommand(UpdateUserDepartmentsRequest request, int currentUserId)
        {
            Request = request;
            CurrentUserId = currentUserId;
        }
    }
}
