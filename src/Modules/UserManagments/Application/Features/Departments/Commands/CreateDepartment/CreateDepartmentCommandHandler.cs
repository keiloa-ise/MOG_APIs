using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MOJ.Modules.UserManagments.Application.Common.Interfaces;
using MOJ.Modules.UserManagments.Application.Features.Departments.DTOs;
using MOJ.Modules.UserManagments.Domain.Entities;
using MOJ.Shared.Application.DTOs;

namespace MOJ.Modules.UserManagments.Application.Features.Departments.Commands.CreateDepartment
{
    public class CreateDepartmentCommandHandler : IRequestHandler<CreateDepartmentCommand, ApiResponse<DepartmentDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<CreateDepartmentCommandHandler> _logger;
        private readonly IDateTime _dateTime;

        public CreateDepartmentCommandHandler(
            IApplicationDbContext context,
            ILogger<CreateDepartmentCommandHandler> logger,
            IDateTime dateTime)
        {
            _context = context;
            _logger = logger;
            _dateTime = dateTime;
        }

        public async Task<ApiResponse<DepartmentDto>> Handle(
            CreateDepartmentCommand command,
            CancellationToken cancellationToken)
        {
            try
            {
                // التحقق من وجود القسم بنفس الكود
                var existingDepartment = await _context.Departments
                    .FirstOrDefaultAsync(d => d.Code == command.Request.Code, cancellationToken);

                if (existingDepartment != null)
                {
                    return ApiResponse<DepartmentDto>.Error(
                        "Department code already exists",
                        new List<string> { $"Department with code '{command.Request.Code}' already exists" });
                }

                // التحقق من وجود القسم الأب (إذا تم تحديده)
                if (command.Request.ParentDepartmentId.HasValue)
                {
                    var parentExists = await _context.Departments
                        .AnyAsync(d => d.Id == command.Request.ParentDepartmentId.Value, cancellationToken);

                    if (!parentExists)
                    {
                        return ApiResponse<DepartmentDto>.Error(
                            "Parent department not found",
                            new List<string> { "Invalid parent department ID" });
                    }
                }

                // إنشاء القسم الجديد
                var department = new Department(
                    command.Request.Name,
                    command.Request.NameAr,
                    command.Request.Code,
                    command.Request.Description,
                    command.Request.ParentDepartmentId
                );

                await _context.Departments.AddAsync(department, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Department created: {DepartmentName} by UserId={UserId}",
                    department.Name, command.CurrentUserId);

                var response = new DepartmentDto
                {
                    Id = department.Id,
                    Name = department.Name,
                    NameAr = department.NameAr,
                    Code = department.Code,
                    Description = department.Description,
                    IsActive = department.IsActive,
                    ParentDepartmentId = department.ParentDepartmentId,
                    CreatedAt = department.CreatedAt,
                    UsersCount = 0
                };

                return ApiResponse<DepartmentDto>.Success(response, "Department created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating department");
                return ApiResponse<DepartmentDto>.Error(
                    "An error occurred while creating department",
                    new List<string> { ex.Message });
            }
        }
    }
}
