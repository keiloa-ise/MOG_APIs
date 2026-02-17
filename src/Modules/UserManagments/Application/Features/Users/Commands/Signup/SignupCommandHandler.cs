using Hangfire;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MOJ.Modules.UserManagments.Application.Common.Interfaces;
using MOJ.Modules.UserManagments.Application.Features.Users.DTOs;
using MOJ.Modules.UserManagments.Domain.Entities;
using MOJ.Shared.Application.DTOs;

namespace MOJ.Modules.UserManagments.Application.Features.Users.Commands.Signup
{
    public class SignupCommandHandler : IRequestHandler<SignupCommand, ApiResponse<UserSignupResponse>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IBackgroundJobClient _backgroundJobClient; // حقن Hangfire
        private readonly ILogger<SignupCommandHandler> _logger;
        private readonly IDateTime _dateTime;

        public SignupCommandHandler(
            IApplicationDbContext context,
            ILogger<SignupCommandHandler> logger,
            IDateTime dateTime, 
            IBackgroundJobClient backgroundJobClient)
        {
            _context = context;
            _logger = logger;
            _dateTime = dateTime;
            _backgroundJobClient = backgroundJobClient;
        }

        public async Task<ApiResponse<UserSignupResponse>> Handle(
            SignupCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                var existingUser = await _context.AppUsers
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u =>
                        u.Email == request.Request.Email ||
                        u.Username == request.Request.Username,
                        cancellationToken);

                if (existingUser != null)
                {
                    var errors = new List<string>();
                    if (existingUser.Email == request.Request.Email)
                        errors.Add("Email already exists");
                    if (existingUser.Username == request.Request.Username)
                        errors.Add("Username already exists");

                    return ApiResponse<UserSignupResponse>.Error(
                        "Registration failed",
                        errors);
                }

                var roleExists = await _context.Roles
                    .AnyAsync(r => r.Id == request.Request.RoleId && r.IsActive, cancellationToken);

                if (!roleExists)
                {
                    return ApiResponse<UserSignupResponse>.Error(
                        "Invalid role specified",
                        new List<string> { $"Role with ID {request.Request.RoleId} does not exist or is inactive" });
                }

                var passwordHash = BCrypt.Net.BCrypt.HashPassword(
                    request.Request.Password,
                    workFactor: 12);

                var user = new AppUser(
                    request.Request.Username,
                    request.Request.Email,
                    passwordHash,
                    request.Request.RoleId,
                    request.Request.FullName,
                    request.Request.PhoneNumber);

                await _context.AppUsers.AddAsync(user, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                var role = await _context.Roles
                    .AsNoTracking()
                    .FirstOrDefaultAsync(r => r.Id == user.RoleId, cancellationToken);

                _logger.LogInformation("User registered successfully: {Email} with role {RoleName}",
                    request.Request.Email, role?.Name);

                // إضافة الأقسام للمستخدم (إذا تم تحديدها)
                if (request.Request.DepartmentIds?.Any() == true)
                {
                    var departments = await _context.Departments
                        .Where(d => request.Request.DepartmentIds.Contains(d.Id) && d.IsActive)
                        .ToListAsync(cancellationToken);

                    foreach (var dept in departments)
                    {
                        var userDepartment = new UserDepartment(
                            user.Id,
                            dept.Id,
                            user.Id, // المستخدم نفسه هو من أضاف نفسه
                            isPrimary: dept.Id == request.Request.PrimaryDepartmentId
                        );
                        await _context.UserDepartments.AddAsync(userDepartment, cancellationToken);
                    }
                    await _context.SaveChangesAsync(cancellationToken);
                }

                var response = new UserSignupResponse
                {
                    UserId = user.Id.ToString(),
                    Username = user.Username,
                    Email = user.Email,
                    FullName = user.FullName,
                    RoleId = user.RoleId,
                    RoleName = role?.Name ?? "Unknown",
                    CreatedAt = user.CreatedAt,
                    Message = "User registered successfully"
                };

                // جدولة مهمة فورية لإرسال إيميل ترحيبي
                _backgroundJobClient.Enqueue<Common.Interfaces.IEmailService>(emailService =>
                    emailService.SendWelcomeEmail(user.Email, user.FullName));

                // جدولة مهمة مؤجلة لإرسال إيميل متابعة بعد 3 أيام
                _backgroundJobClient.Schedule<Common.Interfaces.IEmailService>(emailService =>
                    emailService.SendFollowUpEmail(user.Email), TimeSpan.FromDays(3));

                return ApiResponse<UserSignupResponse>.Success(response, "User registered successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user registration for {Email}", request.Request.Email);
                return ApiResponse<UserSignupResponse>.Error(
                    "An error occurred while processing your request",
                    new List<string> { ex.Message });
            }
        }
    }
}
