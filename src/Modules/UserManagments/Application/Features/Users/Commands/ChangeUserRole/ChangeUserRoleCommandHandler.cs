using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MOJ.Modules.UserManagments.Application.Common.Interfaces;
using MOJ.Modules.UserManagments.Domain.Entities;
using MOJ.Shared.Application.DTOs;

namespace MOJ.Modules.UserManagments.Application.Features.Users.Commands.ChangeUserRole
{
    public class ChangeUserRoleCommandHandler : IRequestHandler<ChangeUserRoleCommand, ApiResponse<ChangeUserRoleResponse>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<ChangeUserRoleCommandHandler> _logger;
        private readonly IDateTime _dateTime;

        public ChangeUserRoleCommandHandler(
            IApplicationDbContext context,
            ILogger<ChangeUserRoleCommandHandler> logger,
            IDateTime dateTime)
        {
            _context = context;
            _logger = logger;
            _dateTime = dateTime;
        }

        public async Task<ApiResponse<ChangeUserRoleResponse>> Handle(
            ChangeUserRoleCommand command,
            CancellationToken cancellationToken)
        {
            var executionStrategy = _context.DbContext.Database.CreateExecutionStrategy();

            return await executionStrategy.ExecuteAsync(async () =>
            {
                using var transaction = await _context.DbContext.Database.BeginTransactionAsync(cancellationToken);

                try
                {
                    // 1. Verify user existence
                    var user = await _context.AppUsers
                        .Include(u => u.Role)
                        .FirstOrDefaultAsync(u => u.Id == command.Request.UserId, cancellationToken);

                    if (user == null)
                    {
                        return ApiResponse<ChangeUserRoleResponse>.Error(
                            "User not found",
                            new List<string> { $"User with ID {command.Request.UserId} does not exist" });
                    }

                    if (!user.IsActive)
                    {
                        return ApiResponse<ChangeUserRoleResponse>.Error(
                            "User is inactive",
                            new List<string> { "Cannot change role for inactive user" });
                    }

                    // 2. Checking for the new role
                    var newRole = await _context.Roles
                        .FirstOrDefaultAsync(r => r.Id == command.Request.NewRoleId, cancellationToken);

                    if (newRole == null)
                    {
                        return ApiResponse<ChangeUserRoleResponse>.Error(
                            "Role not found",
                            new List<string> { $"Role with ID {command.Request.NewRoleId} does not exist" });
                    }

                    if (!newRole.IsActive)
                    {
                        return ApiResponse<ChangeUserRoleResponse>.Error(
                            "Role is inactive",
                            new List<string> { "Cannot assign inactive role to user" });
                    }

                    // 3. Checking the current user's permissions
                    var currentUser = await _context.AppUsers
                        .Include(u => u.Role)
                        .FirstOrDefaultAsync(u => u.Id == command.CurrentUserId, cancellationToken);

                    if (currentUser == null)
                    {
                        return ApiResponse<ChangeUserRoleResponse>.Error(
                            "Unauthorized",
                            new List<string> { "Current user not found" });
                    }

                    // 4. Checking that the user's role has not been changed for themselves
                    if (command.Request.UserId == command.CurrentUserId)
                    {
                        return ApiResponse<ChangeUserRoleResponse>.Error(
                            "Cannot change own role",
                            new List<string> { "You cannot change your own role" });
                    }

                    // 5. Check that the SuperAdmin role has not been changed (Protection)
                    if (user.Role?.Name == Role.DefaultRoles.SuperAdmin)
                    {
                        // Only another SuperAdmin can change the SuperAdmin role.
                        if (currentUser.Role?.Name != Role.DefaultRoles.SuperAdmin)
                        {
                            return ApiResponse<ChangeUserRoleResponse>.Error(
                                "Permission denied",
                                new List<string> { "Cannot change role of SuperAdmin" });
                        }
                    }

                    // 6. Checking role change permissions
                    if (!CanChangeRole(currentUser.Role?.Name, user.Role?.Name, newRole.Name))
                    {
                        return ApiResponse<ChangeUserRoleResponse>.Error(
                            "Permission denied",
                            new List<string> { "You do not have permission to perform this action" });
                    }

                    // 7. Save the previous role
                    var previousRoleId = user.RoleId;
                    var previousRoleName = user.Role?.Name ?? "Unknown";

                    // 8. Change user role
                    user.ChangeRole(command.Request.NewRoleId);
                    user.SetUpdatedAt(_dateTime.UtcNow);

                    _context.AppUsers.Update(user);

                    // 9. Create a change log (Audit Log)
                    var roleChangeLog = new UserRoleChangeLog(
                        userId: user.Id,
                        previousRoleId: previousRoleId,
                        newRoleId: command.Request.NewRoleId,
                        changedByUserId: command.CurrentUserId,
                        reason: command.Request.Reason
                    );

                    roleChangeLog.SetCreatedAt(_dateTime.UtcNow);

                    await _context.UserRoleChangeLogs.AddAsync(roleChangeLog, cancellationToken);

                    // 10. 
                    await _context.SaveChangesAsync(cancellationToken);
                    await transaction.CommitAsync(cancellationToken);

                    // 11. 
                    var response = new ChangeUserRoleResponse
                    {
                        UserId = user.Id.ToString(),
                        Username = user.Username,
                        PreviousRoleId = previousRoleId,
                        PreviousRoleName = previousRoleName,
                        NewRoleId = user.RoleId,
                        NewRoleName = newRole.Name,
                        ChangedBy = currentUser.Username,
                        ChangedAt = _dateTime.UtcNow,
                        Message = "User role changed successfully"
                    };

                    _logger.LogInformation(
                        "User role changed: UserId={UserId}, PreviousRole={PreviousRole}, NewRole={NewRole}, ChangedBy={ChangedBy}",
                        user.Id, previousRoleName, newRole.Name, currentUser.Username);

                    return ApiResponse<ChangeUserRoleResponse>.Success(response, "User role changed successfully");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    _logger.LogError(ex, "Error changing user role for UserId={UserId}", command.Request.UserId);

                    return ApiResponse<ChangeUserRoleResponse>.Error(
                        "An error occurred while changing user role",
                        new List<string> { ex.Message });
                }
            });
        }

        private bool CanChangeRole(string currentUserRole, string targetUserRole, string newRole)
        {
            // Rules of authority for changing roles
            var roleHierarchy = new Dictionary<string, int>
            {
                { Role.DefaultRoles.SuperAdmin, 1 },
                { Role.DefaultRoles.Admin, 2 },
                { Role.DefaultRoles.Manager, 3 },
                { Role.DefaultRoles.Editor, 4 },
                { Role.DefaultRoles.User, 5 },
                { Role.DefaultRoles.Viewer, 6 }
            };

            // 1.SuperAdmin can change any role
            if (currentUserRole == Role.DefaultRoles.SuperAdmin)
                return true;

            // 2. Admin can change any role except SuperAdmin
            if (currentUserRole == Role.DefaultRoles.Admin &&
                targetUserRole != Role.DefaultRoles.SuperAdmin)
                return true;

            // 3. The user can change roles that are below him in the hierarchy
            if (roleHierarchy.ContainsKey(currentUserRole) &&
                roleHierarchy.ContainsKey(targetUserRole) &&
                roleHierarchy.ContainsKey(newRole))
            {
                var currentUserLevel = roleHierarchy[currentUserRole];
                var targetUserLevel = roleHierarchy[targetUserRole];
                var newRoleLevel = roleHierarchy[newRole];

                // The user role can be changed if:
                // a. The current user is higher than the target user
                // b. The new role is higher than or equal to the current user's level
                return currentUserLevel < targetUserLevel && newRoleLevel >= currentUserLevel;
            }

            return false;
        }
    }
}
