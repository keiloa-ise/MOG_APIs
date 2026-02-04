using MediatR;
using MOJ.Shared.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOJ.Modules.UserManagments.Application.Features.Users.Commands.ChangeUserRole
{
    public class ChangeUserRoleCommand : IRequest<ApiResponse<ChangeUserRoleResponse>>
    {
        public ChangeUserRoleRequest Request { get; }
        public int CurrentUserId { get; } 

        public ChangeUserRoleCommand(ChangeUserRoleRequest request, int currentUserId)
        {
            Request = request;
            CurrentUserId = currentUserId;
        }
    }

    public class ChangeUserRoleRequest
    {
        public int UserId { get; set; }
        public int NewRoleId { get; set; }
        public string Reason { get; set; } 
    }

    public class ChangeUserRoleResponse
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string PreviousRoleName { get; set; }
        public int PreviousRoleId { get; set; }
        public string NewRoleName { get; set; }
        public int NewRoleId { get; set; }
        public string ChangedBy { get; set; }
        public DateTime ChangedAt { get; set; }
        public string Message { get; set; }
    }
}
