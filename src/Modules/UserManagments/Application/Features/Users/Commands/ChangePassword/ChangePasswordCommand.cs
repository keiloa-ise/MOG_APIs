using MediatR;
using MOJ.Shared.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOJ.Modules.UserManagments.Application.Features.Users.Commands.ChangePassword
{
    public class ChangePasswordCommand : IRequest<ApiResponse<ChangePasswordResponse>>
    {
        public ChangePasswordRequest Request { get; }
        public int CurrentUserId { get; }

        public ChangePasswordCommand(ChangePasswordRequest request, int currentUserId)
        {
            Request = request;
            CurrentUserId = currentUserId;
        }
    }

    public class ChangePasswordRequest
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }

    public class ChangePasswordResponse
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public DateTime ChangedAt { get; set; }
        public string Message { get; set; }
    }
}
