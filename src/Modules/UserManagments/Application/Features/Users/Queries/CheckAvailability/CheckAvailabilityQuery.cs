using MediatR;
using MOJ.Modules.UserManagments.Application.Features.Users.DTOs;
using MOJ.Shared.Application.DTOs;

namespace MOJ.Modules.UserManagments.Application.Features.Users.Queries.CheckAvailability
{
    public class CheckAvailabilityQuery : IRequest<ApiResponse<CheckAvailabilityResponse>>
    {
        public CheckAvailabilityRequest Request { get; }

        public CheckAvailabilityQuery(CheckAvailabilityRequest request)
        {
            Request = request;
        }
    }
}
