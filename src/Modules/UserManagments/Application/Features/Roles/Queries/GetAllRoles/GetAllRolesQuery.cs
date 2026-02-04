using MediatR;
using MOJ.Modules.UserManagments.Application.Features.Roles.DTOs;
using MOJ.Shared.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOJ.Modules.UserManagments.Application.Features.Roles.Queries.GetAllRoles
{
    public class GetAllRolesQuery : IRequest<ApiResponse<List<RoleDto>>>
    {
        public bool? IsActive { get; set; }

        public GetAllRolesQuery(bool? isActive = null)
        {
            IsActive = isActive;
        }
    }
}
