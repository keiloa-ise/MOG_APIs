using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MOJ.Modules.UserManagments.Application.Common.Interfaces;
using MOJ.Modules.UserManagments.Application.Features.Users.DTOs;
using MOJ.Modules.UserManagments.Domain.Entities;
using MOJ.Shared.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOJ.Modules.UserManagments.Application.Features.Users.Commands.UpdateUserDepartments
{
    public class UpdateUserDepartmentsCommandHandler : IRequestHandler<UpdateUserDepartmentsCommand, ApiResponse<List<UserDepartmentDto>>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<UpdateUserDepartmentsCommandHandler> _logger;
        private readonly IDateTime _dateTime;

        public UpdateUserDepartmentsCommandHandler(
            IApplicationDbContext context,
            ILogger<UpdateUserDepartmentsCommandHandler> logger,
            IDateTime dateTime)
        {
            _context = context;
            _logger = logger;
            _dateTime = dateTime;
        }

        public async Task<ApiResponse<List<UserDepartmentDto>>> Handle(
            UpdateUserDepartmentsCommand command,
            CancellationToken cancellationToken)
        {
            using var transaction = await _context.DbContext.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                // التحقق من وجود المستخدم
                var user = await _context.AppUsers
                    .Include(u => u.UserDepartments)
                    .ThenInclude(ud => ud.Department)
                    .FirstOrDefaultAsync(u => u.Id == command.Request.UserId, cancellationToken);

                if (user == null)
                {
                    return ApiResponse<List<UserDepartmentDto>>.Error(
                        "User not found",
                        new List<string> { $"User with ID {command.Request.UserId} does not exist" });
                }

                // إضافة أقسام جديدة
                if (command.Request.AddDepartmentIds.Any())
                {
                    var departmentsToAdd = await _context.Departments
                        .Where(d => command.Request.AddDepartmentIds.Contains(d.Id) && d.IsActive)
                        .ToListAsync(cancellationToken);

                    foreach (var dept in departmentsToAdd)
                    {
                        // التحقق من عدم وجود القسم مسبقاً
                        if (!user.UserDepartments.Any(ud => ud.DepartmentId == dept.Id))
                        {
                            var userDepartment = new UserDepartment(
                                user.Id,
                                dept.Id,
                                command.CurrentUserId,
                                isPrimary: false
                            );
                            await _context.UserDepartments.AddAsync(userDepartment, cancellationToken);
                        }
                    }
                }

                // حذف أقسام
                if (command.Request.RemoveDepartmentIds.Any())
                {
                    var toRemove = await _context.UserDepartments
                        .Where(ud => ud.UserId == command.Request.UserId &&
                                     command.Request.RemoveDepartmentIds.Contains(ud.DepartmentId))
                        .ToListAsync(cancellationToken);

                    _context.UserDepartments.RemoveRange(toRemove);
                }

                // تحديث القسم الأساسي
                if (command.Request.PrimaryDepartmentId.HasValue)
                {
                    var allUserDepartments = await _context.UserDepartments
                        .Where(ud => ud.UserId == command.Request.UserId)
                        .ToListAsync(cancellationToken);

                    foreach (var ud in allUserDepartments)
                    {
                        if (ud.DepartmentId == command.Request.PrimaryDepartmentId.Value)
                            ud.SetAsPrimary();
                        else
                            ud.RemoveAsPrimary();
                    }
                }

                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                // جلب الأقسام المحدثة
                var updatedDepartments = await _context.UserDepartments
                    .Include(ud => ud.Department)
                    .Include(ud => ud.AssignedByUser)
                    .Where(ud => ud.UserId == command.Request.UserId)
                    .ToListAsync(cancellationToken);

                var response = updatedDepartments.Select(ud => new UserDepartmentDto
                {
                    DepartmentId = ud.DepartmentId,
                    DepartmentName = ud.Department.Name,
                    DepartmentNameAr = ud.Department.NameAr,
                    DepartmentCode = ud.Department.Code,
                    IsPrimary = ud.IsPrimary,
                    AssignedAt = ud.AssignedAt,
                    AssignedBy = ud.AssignedByUser?.Username ?? "System"
                }).ToList();

                _logger.LogInformation("User departments updated for UserId={UserId}", command.Request.UserId);

                return ApiResponse<List<UserDepartmentDto>>.Success(response, "User departments updated successfully");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError(ex, "Error updating departments for UserId={UserId}", command.Request.UserId);
                return ApiResponse<List<UserDepartmentDto>>.Error(
                    "An error occurred while updating departments",
                    new List<string> { ex.Message });
            }
        }
    }
}
