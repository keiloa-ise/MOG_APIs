using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MOJ.Modules.UserManagments.Application.Features.Users.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace MOJ.Modules.UserManagments.Application.Common.Interfaces
{
    public class TokenService : ITokenService
    {
        private readonly IApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly SymmetricSecurityKey _key;
        private readonly RandomNumberGenerator _rng;

        public TokenService(IConfiguration configuration, IApplicationDbContext context)
        {
            _configuration = configuration;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is not configured")));
            _rng = RandomNumberGenerator.Create();
            _context = context;
        }
        public async Task<TokenResponse> GenerateTokens(int userId, string username, string email, string roleName)
        {
            // جلب أقسام المستخدم
            var userDepartments = await _context.UserDepartments
                .Include(ud => ud.Department)
                .Where(ud => ud.UserId == userId)
                .ToListAsync();

            var claims = new List<Claim>
            {
                new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.UniqueName, username),
                new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Email, email),
                new Claim(ClaimTypes.Role, roleName),
                new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };
            // إضافة الأقسام كـ Claims متعددة
            foreach (var ud in userDepartments)
            {
                claims.Add(new Claim("Department", ud.Department.Code));
                claims.Add(new Claim("DepartmentId", ud.DepartmentId.ToString()));

                if (ud.IsPrimary)
                {
                    claims.Add(new Claim("PrimaryDepartment", ud.Department.Code));
                    claims.Add(new Claim("PrimaryDepartmentId", ud.DepartmentId.ToString()));
                }
            }

            // إضافة عدد الأقسام
            claims.Add(new Claim("DepartmentsCount", userDepartments.Count.ToString()));
            var credentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);

            var expires = DateTime.UtcNow.AddMinutes(double.Parse(
                _configuration["Jwt:ExpireMinutes"] ?? "60"));

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: credentials
            );

            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
            var refreshToken = GenerateRefreshToken();

            return new TokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = expires
            };
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            _rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _key,
                ValidateLifetime = false //Important: We check the validity in the main function
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }

        public bool ValidateRefreshToken(string refreshToken)
        {
            // يمكن إضافة منطق للتحقق من صلاحية الـ refresh token
            // مثل التحقق من وجوده في قاعدة البيانات أو Redis
            return !string.IsNullOrEmpty(refreshToken);
        }
    }
}
