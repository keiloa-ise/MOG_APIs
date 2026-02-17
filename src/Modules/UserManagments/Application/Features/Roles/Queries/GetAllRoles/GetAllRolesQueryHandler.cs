using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MOJ.Modules.UserManagments.Application.Common.Interfaces;
using MOJ.Modules.UserManagments.Application.Features.Roles.DTOs;
using MOJ.Shared.Application.DTOs;

namespace MOJ.Modules.UserManagments.Application.Features.Roles.Queries.GetAllRoles
{
    public class GetAllRolesQueryHandler : IRequestHandler<GetAllRolesQuery, ApiResponse<List<RoleDto>>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<GetAllRolesQueryHandler> _logger;

        public GetAllRolesQueryHandler(
            IApplicationDbContext context,
            ILogger<GetAllRolesQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ApiResponse<List<RoleDto>>> Handle(
            GetAllRolesQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var query = _context.Roles.AsQueryable();

                // Filter by activity
                if (request.IsActive.HasValue)
                {
                    query = query.Where(r => r.IsActive == request.IsActive.Value);
                }

                var roles = await query
                    .Select(r => new RoleDto
                    {
                        Id = r.Id,
                        Name = r.Name,
                        Description = r.Description,
                        IsActive = r.IsActive,
                        CreatedAt = r.CreatedAt,
                        UserCount = r.Users.Count(u => u.IsActive)
                    })
                    .OrderBy(r => r.Name)
                    .ToListAsync(cancellationToken);

                _logger.LogInformation("Retrieved {Count} roles", roles.Count);

                return ApiResponse<List<RoleDto>>.Success(roles, "Roles retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving roles");
                return ApiResponse<List<RoleDto>>.Error(
                    "An error occurred while retrieving roles",
                    new List<string> { ex.Message });
            }
        }
    }
}
