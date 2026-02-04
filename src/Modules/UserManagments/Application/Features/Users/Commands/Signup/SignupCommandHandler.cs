using BCrypt.Net;
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
        private readonly ILogger<SignupCommandHandler> _logger;
        private readonly IDateTime _dateTime;

        public SignupCommandHandler(
            IApplicationDbContext context,
            ILogger<SignupCommandHandler> logger,
            IDateTime dateTime)
        {
            _context = context;
            _logger = logger;
            _dateTime = dateTime;
        }

        public async Task<ApiResponse<UserSignupResponse>> Handle(
            SignupCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                // التحقق من وجود المستخدم
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

                // التحقق من وجود الـ Role
                var roleExists = await _context.Roles
                    .AnyAsync(r => r.Id == request.Request.RoleId && r.IsActive, cancellationToken);

                if (!roleExists)
                {
                    return ApiResponse<UserSignupResponse>.Error(
                        "Invalid role specified",
                        new List<string> { $"Role with ID {request.Request.RoleId} does not exist or is inactive" });
                }

                // تشفير كلمة المرور
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(
                    request.Request.Password,
                    workFactor: 12);

                // إنشاء المستخدم الجديد
                var user = new AppUser(
                    request.Request.Username,
                    request.Request.Email,
                    passwordHash,
                    request.Request.RoleId, // استخدام RoleId
                    request.Request.FullName,
                    request.Request.PhoneNumber);

                // حفظ المستخدم
                await _context.AppUsers.AddAsync(user, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                // جلب بيانات الـ Role
                var role = await _context.Roles
                    .AsNoTracking()
                    .FirstOrDefaultAsync(r => r.Id == user.RoleId, cancellationToken);

                _logger.LogInformation("User registered successfully: {Email} with role {RoleName}",
                    request.Request.Email, role?.Name);

                // إعداد الاستجابة
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
