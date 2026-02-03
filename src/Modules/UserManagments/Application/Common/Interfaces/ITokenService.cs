using MOJ.Modules.UserManagments.Application.Features.Users.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MOJ.Modules.UserManagments.Application.Common.Interfaces
{
    public interface ITokenService
    {
        TokenResponse GenerateTokens(int userId, string username, string email, string role);
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
        string GenerateRefreshToken();
        bool ValidateRefreshToken(string refreshToken);
    }
}
