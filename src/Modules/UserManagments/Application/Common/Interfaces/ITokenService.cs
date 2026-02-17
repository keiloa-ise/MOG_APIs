using MOJ.Modules.UserManagments.Application.Features.Users.DTOs;
using System.Security.Claims;

namespace MOJ.Modules.UserManagments.Application.Common.Interfaces
{
    public interface ITokenService
    {
        Task<TokenResponse> GenerateTokens(int userId, string username, string email, string roleName);
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
        string GenerateRefreshToken();
        bool ValidateRefreshToken(string refreshToken);
    }
}
