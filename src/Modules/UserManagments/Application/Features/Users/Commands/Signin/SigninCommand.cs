using MediatR;
using MOJ.Modules.UserManagments.Application.Features.Users.DTOs;
using MOJ.Shared.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOJ.Modules.UserManagments.Application.Features.Users.Commands.Signin
{
    public class SigninCommand : IRequest<ApiResponse<SigninResponse>>
    {
        public SigninRequest Request { get; }

        public SigninCommand(SigninRequest request)
        {
            Request = request;
        }
    }
}
