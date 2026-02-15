using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MOJ.Modules.UserManagments.Application.Common.Interfaces;
using MOJ.Modules.UserManagments.Application.Features.Users.DTOs;
using MOJ.Shared.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOJ.Modules.UserManagments.Application.Features.Users.Queries.GetUserDepartments
{
    public class GetUserDepartmentsQueryHandler : IRequestHandler<GetUserDepartmentsQuery, ApiResponse<List<UserDepartmentDto>>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<GetUserDepartmentsQueryHandler> _logger;

        public GetUserDepartmentsQueryHandler(
            IApplicationDbContext context,
            ILogger<GetUserDepartmentsQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ApiResponse<List<UserDepartmentDto>>> Handle(
            GetUserDepartmentsQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var userDepartments = await _context.UserDepartments
                    .Include(ud => ud.Department)
                    .Include(ud => ud.AssignedByUser)
                    .Where(ud => ud.UserId == request.UserId)
                    .OrderByDescending(ud => ud.IsPrimary)
                    .ThenBy(ud => ud.Department.Name)
                    .ToListAsync(cancellationToken);

                var response = userDepartments.Select(ud => new UserDepartmentDto
                {
                    DepartmentId = ud.DepartmentId,
                    DepartmentName = ud.Department.Name,
                    DepartmentNameAr = ud.Department.NameAr,
                    DepartmentCode = ud.Department.Code,
                    IsPrimary = ud.IsPrimary,
                    AssignedAt = ud.AssignedAt,
                    AssignedBy = ud.AssignedByUser?.Username ?? "System"
                }).ToList();

                return ApiResponse<List<UserDepartmentDto>>.Success(response, "User departments retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving departments for UserId={UserId}", request.UserId);
                return ApiResponse<List<UserDepartmentDto>>.Error(
                    "An error occurred while retrieving user departments",
                    new List<string> { ex.Message });
            }
        }
    }
}
