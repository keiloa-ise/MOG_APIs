using MediatR;
using MOJ.Shared.Application.DTOs;

namespace MOJ.Modules.UserManagments.Application.Features.Users.Queries.GetUserRoleHistory
{
    public class GetUserRoleHistoryQuery : IRequest<ApiResponse<List<UserRoleHistoryResponse>>>
    {
        public int UserId { get; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public GetUserRoleHistoryQuery(int userId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            UserId = userId;
            FromDate = fromDate;
            ToDate = toDate;
        }
    }

    public class UserRoleHistoryResponse
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
        public string PreviousRoleName { get; set; }
        public string NewRoleName { get; set; }
        public string ChangedByUsername { get; set; }
        public string Reason { get; set; }
        public DateTime ChangedAt { get; set; }
    }
}
