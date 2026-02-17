using FluentValidation;

namespace MOJ.Modules.UserManagments.Application.Features.Users.Commands.AssignDepartments
{

    public class AssignDepartmentsCommandValidator : AbstractValidator<AssignDepartmentsCommand>
    {
        public AssignDepartmentsCommandValidator()
        {
            // التحقق من UserId عبر Request
            RuleFor(x => x.Request.UserId)
                .GreaterThan(0).WithMessage("Invalid user ID");

            // التحقق من DepartmentIds عبر Request
            RuleFor(x => x.Request.DepartmentIds)
                .NotNull().WithMessage("Department IDs cannot be null")
                .NotEmpty().WithMessage("At least one department must be selected");

            // التحقق من عدم وجود تكرار في DepartmentIds
            RuleFor(x => x.Request.DepartmentIds)
                .Must(ids => ids == null || ids.Count == ids.Distinct().Count())
                .WithMessage("Duplicate department IDs are not allowed");

            // التحقق من PrimaryDepartmentId
            RuleFor(x => x.Request.PrimaryDepartmentId)
                .Must((command, primaryId) =>
                    !primaryId.HasValue ||
                    (command.Request.DepartmentIds != null &&
                     command.Request.DepartmentIds.Contains(primaryId.Value)))
                .WithMessage("Primary department must be one of the selected departments");

            // التحقق من CurrentUserId
            RuleFor(x => x.CurrentUserId)
                .GreaterThan(0).WithMessage("Invalid current user ID");

            // التحقق من عدم تعيين الأقسام للمستخدم نفسه
            RuleFor(x => x)
                .Must(x => x.CurrentUserId != x.Request.UserId)
                .When(x => x.Request.UserId > 0 && x.CurrentUserId > 0)
                .WithMessage("You cannot assign departments to yourself");

            // التحقق من أن PrimaryDepartmentId موجود في القائمة إذا تم توفيره
            RuleFor(x => x)
                .Must(x =>
                    !x.Request.PrimaryDepartmentId.HasValue ||
                    (x.Request.DepartmentIds != null &&
                     x.Request.DepartmentIds.Contains(x.Request.PrimaryDepartmentId.Value)))
                .WithMessage("Primary department must be in the list of assigned departments");

            // التحقق من أن هناك على الأقل قسم واحد إذا تم تحديد PrimaryDepartmentId
            RuleFor(x => x.Request.DepartmentIds)
                .Must((command, ids) =>
                    !command.Request.PrimaryDepartmentId.HasValue ||
                    (ids != null && ids.Any()))
                .WithMessage("Cannot set primary department when no departments are selected");
        }
    }
}
