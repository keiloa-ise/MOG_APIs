using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MOJ.Modules.UserManagments.Application.Common.Interfaces;
using MOJ.Modules.UserManagments.Application.Features.Users.DTOs;
using MOJ.Modules.UserManagments.Domain.Entities;
using MOJ.Shared.Application.DTOs;

namespace MOJ.Modules.UserManagments.Application.Features.Users.Commands.AssignDepartments
{
    public class AssignDepartmentsCommandHandler : IRequestHandler<AssignDepartmentsCommand, ApiResponse<List<UserDepartmentDto>>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<AssignDepartmentsCommandHandler> _logger;
        private readonly IDateTime _dateTime;

        public AssignDepartmentsCommandHandler(
            IApplicationDbContext context,
            ILogger<AssignDepartmentsCommandHandler> logger,
            IDateTime dateTime)
        {
            _context = context;
            _logger = logger;
            _dateTime = dateTime;
        }

        public async Task<ApiResponse<List<UserDepartmentDto>>> Handle(
            AssignDepartmentsCommand command,
            CancellationToken cancellationToken)
        {
            //  استخدام Execution Strategy
            var executionStrategy = _context.DbContext.Database.CreateExecutionStrategy();

            return await executionStrategy.ExecuteAsync(async () =>
            {
                // بدأ transaction داخل Execution Strategy
                using var transaction = await _context.DbContext.Database.BeginTransactionAsync(cancellationToken);

                try
                {
                    // التحقق من وجود المستخدم
                    var user = await _context.AppUsers
                        .Include(u => u.UserDepartments)
                        .FirstOrDefaultAsync(u => u.Id == command.Request.UserId, cancellationToken);

                    if (user == null)
                    {
                        return ApiResponse<List<UserDepartmentDto>>.Error(
                            "User not found",
                            new List<string> { $"User with ID {command.Request.UserId} does not exist" });
                    }

                    // التحقق من وجود الأقسام
                    var departments = await _context.Departments
                        .Where(d => command.Request.DepartmentIds.Contains(d.Id) && d.IsActive)
                        .ToListAsync(cancellationToken);

                    var missingDepartments = command.Request.DepartmentIds
                        .Except(departments.Select(d => d.Id))
                        .ToList();

                    if (missingDepartments.Any())
                    {
                        return ApiResponse<List<UserDepartmentDto>>.Error(
                            "Invalid departments",
                            new List<string> { $"Departments not found or inactive: {string.Join(", ", missingDepartments)}" });
                    }

                    // حذف الأقسام القديمة
                    var existingAssignments = await _context.UserDepartments
                        .Where(ud => ud.UserId == command.Request.UserId)
                        .ToListAsync(cancellationToken);

                    if (existingAssignments.Any())
                    {
                        _context.UserDepartments.RemoveRange(existingAssignments);
                    }

                    // إضافة الأقسام الجديدة
                    var newAssignments = new List<UserDepartment>();
                    foreach (var deptId in command.Request.DepartmentIds)
                    {
                        var isPrimary = command.Request.PrimaryDepartmentId == deptId;

                        var userDepartment = new UserDepartment(
                            command.Request.UserId,
                            deptId,
                            command.CurrentUserId,
                            isPrimary
                        );

                        newAssignments.Add(userDepartment);
                    }

                    // التأكد من وجود قسم أساسي واحد على الأقل
                    if (command.Request.DepartmentIds.Any() && !command.Request.PrimaryDepartmentId.HasValue)
                    {
                        // تعيين أول قسم كقسم أساسي
                        newAssignments.First().SetAsPrimary();
                    }

                    await _context.UserDepartments.AddRangeAsync(newAssignments, cancellationToken);
                    await _context.SaveChangesAsync(cancellationToken);
                    await transaction.CommitAsync(cancellationToken);

                    // جلب اسم المسؤول بشكل منفصل
                    var assignedByUser = await _context.AppUsers
                        .Where(u => u.Id == command.CurrentUserId)
                        .Select(u => u.Username)
                        .FirstOrDefaultAsync(cancellationToken);

                    var assignedByName = assignedByUser ?? "System";

                    // إعداد الاستجابة
                    var response = newAssignments.Select(ud =>
                    {
                        var department = departments.First(d => d.Id == ud.DepartmentId);
                        return new UserDepartmentDto
                        {
                            DepartmentId = ud.DepartmentId,
                            DepartmentName = department.Name,
                            DepartmentNameAr = department.NameAr,
                            DepartmentCode = department.Code,
                            IsPrimary = ud.IsPrimary,
                            AssignedAt = ud.AssignedAt,
                            AssignedBy = assignedByName
                        };
                    }).ToList();

                    _logger.LogInformation("Departments assigned to UserId={UserId} by AdminId={AdminId}",
                        command.Request.UserId, command.CurrentUserId);

                    return ApiResponse<List<UserDepartmentDto>>.Success(response, "Departments assigned successfully");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    _logger.LogError(ex, "Error assigning departments to UserId={UserId}", command.Request.UserId);
                    return ApiResponse<List<UserDepartmentDto>>.Error(
                        "An error occurred while assigning departments",
                        new List<string> { ex.Message });
                }
            });
        }
    }
}