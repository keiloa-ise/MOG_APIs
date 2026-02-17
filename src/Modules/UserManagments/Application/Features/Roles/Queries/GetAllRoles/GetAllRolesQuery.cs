using MediatR;
using MOJ.Modules.UserManagments.Application.Features.Roles.DTOs;
using MOJ.Shared.Application.DTOs;

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
