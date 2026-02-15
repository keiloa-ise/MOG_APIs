using MediatR;
using MOJ.Modules.UserManagments.Application.Features.Departments.DTOs;
using MOJ.Shared.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOJ.Modules.UserManagments.Application.Features.Departments.Commands.CreateDepartment
{
    public class CreateDepartmentCommand : IRequest<ApiResponse<DepartmentDto>>
    {
        public CreateDepartmentRequest Request { get; }
        public int CurrentUserId { get; }

        public CreateDepartmentCommand(CreateDepartmentRequest request, int currentUserId)
        {
            Request = request;
            CurrentUserId = currentUserId;
        }
    }
}
