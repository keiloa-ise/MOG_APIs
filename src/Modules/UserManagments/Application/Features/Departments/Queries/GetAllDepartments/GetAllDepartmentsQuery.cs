using MediatR;
using MOJ.Modules.UserManagments.Application.Features.Departments.DTOs;
using MOJ.Shared.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOJ.Modules.UserManagments.Application.Features.Departments.Queries.GetAllDepartments
{
    public class GetAllDepartmentsQuery : IRequest<ApiResponse<List<DepartmentDto>>>
    {
        public bool? IsActive { get; set; }
        public bool IncludeHierarchy { get; set; }

        public GetAllDepartmentsQuery(bool? isActive = null, bool includeHierarchy = false)
        {
            IsActive = isActive;
            IncludeHierarchy = includeHierarchy;
        }
    }
}
