using FluentValidation;
using Microsoft.EntityFrameworkCore;
using MOJ.Modules.UserManagments.Application.Common.Interfaces;
using MOJ.Modules.UserManagments.Application.Features.Users.DTOs;

namespace MOJ.Modules.UserManagments.Application.Features.Users.Commands.UpdateUserDepartments
{
    public class UpdateUserDepartmentsCommandValidator : AbstractValidator<UpdateUserDepartmentsCommand>
    {
        private readonly IApplicationDbContext _context;

        public UpdateUserDepartmentsCommandValidator(IApplicationDbContext context)
        {
            _context = context;

            RuleFor(x => x.Request.UserId)
                .GreaterThan(0).WithMessage("Invalid user ID")
                .MustAsync(UserExists).WithMessage("User does not exist");

            RuleFor(x => x.Request.AddDepartmentIds)
                .Must(ids => ids == null || ids.Count == ids.Distinct().Count())
                .WithMessage("Duplicate department IDs in add list are not allowed")
                .MustAsync(async (ids, cancellationToken) =>
                    await DepartmentsExist(ids, cancellationToken))
                .WithMessage("One or more departments to add do not exist or are inactive");

            RuleFor(x => x.Request.RemoveDepartmentIds)
                .Must(ids => ids == null || ids.Count == ids.Distinct().Count())
                .WithMessage("Duplicate department IDs in remove list are not allowed")
                .MustAsync(async (ids, cancellationToken) =>
                    await DepartmentsExist(ids, cancellationToken))
                .WithMessage("One or more departments to remove do not exist");

            RuleFor(x => x.Request.AddDepartmentIds)
                .Must((request, addIds) =>
                    addIds == null ||
                    request.Request.RemoveDepartmentIds == null ||
                    !addIds.Intersect(request.Request.RemoveDepartmentIds).Any())
                .WithMessage("Cannot add and remove the same department");

            RuleFor(x => x.Request.PrimaryDepartmentId)
                .MustAsync(async (request, primaryId, cancellationToken) =>
                {
                    if (!primaryId.HasValue) return true;

                    var finalDepartments = await GetFinalDepartments(request.Request, cancellationToken);
                    return finalDepartments.Contains(primaryId.Value);
                })
                .WithMessage("Primary department must be one of the user's departments after update");

            RuleFor(x => x.CurrentUserId)
                .GreaterThan(0).WithMessage("Invalid current user ID")
                .NotEqual(x => x.Request.UserId)
                .When(x => x.Request.UserId > 0)
                .WithMessage("You cannot update your own departments");
        }

        private async Task<bool> UserExists(int userId, CancellationToken cancellationToken)
        {
            return await _context.AppUsers
                .AnyAsync(u => u.Id == userId, cancellationToken);
        }

        private async Task<bool> DepartmentsExist(List<int> departmentIds, CancellationToken cancellationToken)
        {
            if (departmentIds == null || !departmentIds.Any())
                return true;

            var existingCount = await _context.Departments
                .CountAsync(d => departmentIds.Contains(d.Id), cancellationToken);

            return existingCount == departmentIds.Count;
        }

        private async Task<List<int>> GetFinalDepartments(UpdateUserDepartmentsRequest request, CancellationToken cancellationToken)
        {
            // الحصول على الأقسام الحالية للمستخدم
            var currentDepartments = await _context.UserDepartments
                .Where(ud => ud.UserId == request.UserId)
                .Select(ud => ud.DepartmentId)
                .ToListAsync(cancellationToken);

            var finalDepartments = new List<int>(currentDepartments);

            // إضافة الأقسام الجديدة
            if (request.AddDepartmentIds != null)
            {
                finalDepartments.AddRange(request.AddDepartmentIds);
            }

            // إزالة الأقسام المحددة
            if (request.RemoveDepartmentIds != null)
            {
                finalDepartments = finalDepartments
                    .Where(id => !request.RemoveDepartmentIds.Contains(id))
                    .ToList();
            }

            return finalDepartments.Distinct().ToList();
        }
    }
}
