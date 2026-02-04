using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MOJ.Modules.UserManagments.Application.Common.Interfaces;
using MOJ.Shared.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOJ.Modules.UserManagments.Application.Features.Users.Queries.GetUserRoleHistory
{
    public class GetUserRoleHistoryQueryHandler :
        IRequestHandler<GetUserRoleHistoryQuery, ApiResponse<List<UserRoleHistoryResponse>>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<GetUserRoleHistoryQueryHandler> _logger;

        public GetUserRoleHistoryQueryHandler(
            IApplicationDbContext context,
            ILogger<GetUserRoleHistoryQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ApiResponse<List<UserRoleHistoryResponse>>> Handle(
            GetUserRoleHistoryQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                // بدء الـ Query
                var query = _context.UserRoleChangeLogs
                    .Include(l => l.User)
                    .Include(l => l.PreviousRole)
                    .Include(l => l.NewRole)
                    .Include(l => l.ChangedByUser)
                    .Where(l => l.UserId == request.UserId);

                // تطبيق الفلاتر التاريخية
                if (request.FromDate.HasValue)
                {
                    query = query.Where(l => l.CreatedAt >= request.FromDate.Value);
                }

                if (request.ToDate.HasValue)
                {
                    query = query.Where(l => l.CreatedAt <= request.ToDate.Value);
                }

                // التطبيق النهائي للترتيب والـ projection
                var history = await query
                    .OrderByDescending(l => l.CreatedAt)
                    .Select(l => new UserRoleHistoryResponse
                    {
                        Id = l.Id,
                        UserId = l.UserId,
                        Username = l.User.Username,
                        PreviousRoleName = l.PreviousRole.Name,
                        NewRoleName = l.NewRole.Name,
                        ChangedByUsername = l.ChangedByUser.Username,
                        Reason = l.Reason,
                        ChangedAt = l.CreatedAt
                    })
                    .ToListAsync(cancellationToken);

                _logger.LogInformation("Retrieved role history for UserId={UserId}, Count={Count}",
                    request.UserId, history.Count);

                return ApiResponse<List<UserRoleHistoryResponse>>.Success(
                    history,
                    "Role history retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving role history for UserId={UserId}",
                    request.UserId);

                return ApiResponse<List<UserRoleHistoryResponse>>.Error(
                    "An error occurred while retrieving role history",
                    new List<string> { ex.Message });
            }
        }
    }
}
