using BCrypt.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MOJ.Modules.UserManagments.Application.Common.Interfaces;
using MOJ.Modules.UserManagments.Application.Features.Users.DTOs;
using MOJ.Modules.UserManagments.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                //
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

                // 
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(
                    request.Request.Password,
                    workFactor: 12);

                // 
                var user = new AppUser(
                    request.Request.Username,
                    request.Request.Email,
                    passwordHash,
                    request.Request.FullName,
                    request.Request.PhoneNumber);

                // 
                await _context.AppUsers.AddAsync(user, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("User registered successfully: {Email}", request.Request.Email);

                // 
                var response = new UserSignupResponse
                {
                    UserId = user.Id.ToString(),
                    Username = user.Username,
                    Email = user.Email,
                    FullName = user.FullName,
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
