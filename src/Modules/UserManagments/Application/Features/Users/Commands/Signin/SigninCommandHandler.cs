using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MOJ.Modules.UserManagments.Application.Common.Interfaces;
using MOJ.Modules.UserManagments.Application.Features.Users.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOJ.Modules.UserManagments.Application.Features.Users.Commands.Signin
{
    public class SigninCommandHandler : IRequestHandler<SigninCommand, ApiResponse<SigninResponse>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly ILogger<SigninCommandHandler> _logger;
        private readonly IDateTime _dateTime;

        public SigninCommandHandler(
            IApplicationDbContext context,
            ITokenService tokenService,
            ILogger<SigninCommandHandler> logger,
            IDateTime dateTime)
        {
            _context = context;
            _tokenService = tokenService;
            _logger = logger;
            _dateTime = dateTime;
        }

        public async Task<ApiResponse<SigninResponse>> Handle(
            SigninCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                // Search for a user using their username or email address.
                var user = await _context.AppUsers
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u =>
                        u.Username == request.Request.UsernameOrEmail ||
                        u.Email == request.Request.UsernameOrEmail,
                        cancellationToken);

                if (user == null)
                {
                    _logger.LogWarning("User not found: {Identifier}", request.Request.UsernameOrEmail);
                    return ApiResponse<SigninResponse>.Error(
                        "Invalid username/email or password",
                        new List<string> { "Authentication failed" });
                }

                if (!user.IsActive)
                {
                    _logger.LogWarning("Inactive user attempted login: {UserId}", user.Id);
                    return ApiResponse<SigninResponse>.Error(
                        "Account is deactivated",
                        new List<string> { "Please contact administrator" });
                }

                // Password verification
                var isValidPassword = BCrypt.Net.BCrypt.Verify(
                    request.Request.Password,
                    user.PasswordHash);

                if (!isValidPassword)
                {
                    _logger.LogWarning("Invalid password for user: {UserId}", user.Id);
                    return ApiResponse<SigninResponse>.Error(
                        "Invalid username/email or password",
                        new List<string> { "Authentication failed" });
                }

                // Last login time updated
                user.UpdateLastLogin();
                _context.AppUsers.Update(user);
                await _context.SaveChangesAsync(cancellationToken);

                // Create tokens
                var tokens = _tokenService.GenerateTokens(
                    user.Id,
                    user.Username,
                    user.Email,
                    user.Role);

                // 
                var response = new SigninResponse
                {
                    UserId = user.Id.ToString(),
                    Username = user.Username,
                    Email = user.Email,
                    FullName = user.FullName,
                    Role = user.Role,
                    AccessToken = tokens.AccessToken,
                    RefreshToken = tokens.RefreshToken,
                    TokenExpiry = tokens.ExpiresAt,
                    LastLogin = user.LastLogin,
                    Message = "Login successful"
                };

                _logger.LogInformation("User logged in successfully: {UserId}", user.Id);

                return ApiResponse<SigninResponse>.Success(response, "Login successful");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user signin for {Identifier}",
                    request.Request.UsernameOrEmail);

                return ApiResponse<SigninResponse>.Error(
                    "An error occurred while processing your request",
                    new List<string> { ex.Message });
            }
        }
    }
}
