using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MOJ.Modules.UserManagments.Application.Common.Interfaces;
using MOJ.Modules.UserManagments.Application.Features.Departments.DTOs;
using MOJ.Shared.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOJ.Modules.UserManagments.Application.Features.Departments.Queries.GetAllDepartments
{
    public class GetAllDepartmentsQueryHandler : IRequestHandler<GetAllDepartmentsQuery, ApiResponse<List<DepartmentDto>>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<GetAllDepartmentsQueryHandler> _logger;

        public GetAllDepartmentsQueryHandler(
            IApplicationDbContext context,
            ILogger<GetAllDepartmentsQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ApiResponse<List<DepartmentDto>>> Handle(
            GetAllDepartmentsQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var query = _context.Departments
                    .Include(d => d.UserDepartments)
                    .AsQueryable();

                if (request.IsActive.HasValue)
                {
                    query = query.Where(d => d.IsActive == request.IsActive.Value);
                }

                if (request.IncludeHierarchy)
                {
                    // جلب جميع الأقسام مع العلاقات
                    var departments = await query
                        .OrderBy(d => d.ParentDepartmentId)
                        .ToListAsync(cancellationToken);

                    var departmentDtos = BuildHierarchy(departments, null);
                    return ApiResponse<List<DepartmentDto>>.Success(departmentDtos, "Departments retrieved successfully");
                }
                else
                {
                    // جلب الأقسام المسطحة
                    var departments = await query
                        .Select(d => new DepartmentDto
                        {
                            Id = d.Id,
                            Name = d.Name,
                            NameAr = d.NameAr,
                            Code = d.Code,
                            Description = d.Description,
                            IsActive = d.IsActive,
                            ParentDepartmentId = d.ParentDepartmentId,
                            CreatedAt = d.CreatedAt,
                            UsersCount = d.UserDepartments.Count
                        })
                        .ToListAsync(cancellationToken);

                    return ApiResponse<List<DepartmentDto>>.Success(departments, "Departments retrieved successfully");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving departments");
                return ApiResponse<List<DepartmentDto>>.Error(
                    "An error occurred while retrieving departments",
                    new List<string> { ex.Message });
            }
        }

        private List<DepartmentDto> BuildHierarchy(List<Domain.Entities.Department> allDepartments, int? parentId)
        {
            return allDepartments
                .Where(d => d.ParentDepartmentId == parentId)
                .Select(d => new DepartmentDto
                {
                    Id = d.Id,
                    Name = d.Name,
                    NameAr = d.NameAr,
                    Code = d.Code,
                    Description = d.Description,
                    IsActive = d.IsActive,
                    ParentDepartmentId = d.ParentDepartmentId,
                    CreatedAt = d.CreatedAt,
                    UsersCount = d.UserDepartments.Count,
                    ChildDepartments = BuildHierarchy(allDepartments, d.Id)
                })
                .ToList();
        }
    }
}
