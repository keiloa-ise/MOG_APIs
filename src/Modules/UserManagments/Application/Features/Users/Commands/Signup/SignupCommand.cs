using MediatR;
using MOJ.Modules.UserManagments.Application.Features.Users.DTOs;
using MOJ.Shared.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOJ.Modules.UserManagments.Application.Features.Users.Commands.Signup
{
    public class SignupCommand : IRequest<ApiResponse<UserSignupResponse>>
    {
        public UserSignupRequest Request { get; }

        public SignupCommand(UserSignupRequest request)
        {
            Request = request;
        }
    }
}
