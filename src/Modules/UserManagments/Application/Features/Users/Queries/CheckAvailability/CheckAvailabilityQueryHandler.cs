using MediatR;
using Microsoft.Extensions.Logging;
using MOJ.Modules.UserManagments.Application.Features.Users.DTOs;
using MOJ.Modules.UserManagments.Application.Features.Users.Queries.CheckAvailability;
using MOJ.Modules.UserManagments.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Users.Queries.CheckAvailability
{
    public class CheckAvailabilityQueryHandler : IRequestHandler<CheckAvailabilityQuery, ApiResponse<CheckAvailabilityResponse>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<CheckAvailabilityQueryHandler> _logger;

        public CheckAvailabilityQueryHandler(
            IApplicationDbContext context,
            ILogger<CheckAvailabilityQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ApiResponse<CheckAvailabilityResponse>> Handle(
            CheckAvailabilityQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var email = request.Request.Email?.Trim();
                var username = request.Request.Username?.Trim();

                // 
                var userExists = await _context.AppUsers
                    .AsNoTracking()
                    .AnyAsync(u =>
                        (email != null && u.Email == email) ||
                        (username != null && u.Username == username),
                        cancellationToken);

                var message = userExists ? "Already taken" : "Available";

                var response = new CheckAvailabilityResponse
                {
                    Available = !userExists,
                    Email = email,
                    Username = username,
                    Message = message
                };

                return ApiResponse<CheckAvailabilityResponse>.Success(response, "Availability checked successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking availability");
                return ApiResponse<CheckAvailabilityResponse>.Error(
                    "An error occurred while checking availability",
                    new List<string> { ex.Message });
            }
        }
    }
}
