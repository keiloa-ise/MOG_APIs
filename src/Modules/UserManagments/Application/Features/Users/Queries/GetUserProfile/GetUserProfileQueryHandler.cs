using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MOJ.Modules.UserManagments.Application.Common.Interfaces;
using MOJ.Modules.UserManagments.Application.Features.Users.DTOs;
using MOJ.Shared.Application.DTOs;

namespace MOJ.Modules.UserManagments.Application.Features.Users.Queries.GetUserProfile
{
    public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, ApiResponse<UserProfileResponse>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<GetUserProfileQueryHandler> _logger;

        public GetUserProfileQueryHandler(
            IApplicationDbContext context,
            ILogger<GetUserProfileQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ApiResponse<UserProfileResponse>> Handle(
            GetUserProfileQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var user = await _context.AppUsers
                    .Include(u => u.Role) // Include Role data
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

                if (user == null)
                {
                    return ApiResponse<UserProfileResponse>.Error(
                        "User not found",
                        new List<string> { "User does not exist" });
                }

                var response = new UserProfileResponse
                {
                    UserId = user.Id.ToString(),
                    Username = user.Username,
                    Email = user.Email,
                    FullName = user.FullName,
                    PhoneNumber = user.PhoneNumber,
                    RoleId = user.RoleId,
                    RoleName = user.Role?.Name ?? "Unknown", // Get Role name
                    CreatedAt = user.CreatedAt,
                    LastLogin = user.LastLogin,
                    IsActive = user.IsActive
                };

                return ApiResponse<UserProfileResponse>.Success(response, "Profile retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user profile for {UserId}", request.UserId);
                return ApiResponse<UserProfileResponse>.Error(
                    "An error occurred while retrieving profile",
                    new List<string> { ex.Message });
            }
        }
    }
}
