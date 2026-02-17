using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MOJ.Modules.UserManagments.Application.Features.Users.Commands.ChangePassword;
using MOJ.Modules.UserManagments.Application.Features.Users.Commands.ChangeUserRole;
using MOJ.Modules.UserManagments.Application.Features.Users.Commands.Signin;
using MOJ.Modules.UserManagments.Application.Features.Users.Commands.Signup;
using MOJ.Modules.UserManagments.Application.Features.Users.DTOs;
using MOJ.Modules.UserManagments.Application.Features.Users.Queries.CheckAvailability;
using MOJ.Modules.UserManagments.Application.Features.Users.Queries.GetUserProfile;
using MOJ.Modules.UserManagments.Application.Features.Users.Queries.GetUserRoleHistory;
using MOJ.Shared.Application;
using MOJ.Shared.Application.DTOs;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MOJ.Modules.UserManagments.API.Controllers
{
    [Route("api/users")]
    public class UserManagmentController : ApiControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<UserManagmentController> _logger;
        public UserManagmentController(IMediator mediator, ILogger<UserManagmentController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }
        [Authorize]
        [HttpPost("change-password")]
        [ProducesResponseType(typeof(ApiResponse<ChangePasswordResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            try
            {
                // الحصول على ID المستخدم الحالي من الـ token
                var currentUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    ?? User.FindFirst("sub")?.Value;

                if (string.IsNullOrEmpty(currentUserIdClaim) || !int.TryParse(currentUserIdClaim, out int currentUserId))
                {
                    return BadRequest(new
                    {
                        Status = "error",
                        Message = "Invalid authentication token"
                    });
                }

                var command = new ChangePasswordCommand(request, currentUserId);
                var result = await _mediator.Send(command);

                if (result.Status == "error")
                {
                    return BadRequest(new
                    {
                        result.Status,
                        result.Message,
                        result.Errors
                    });
                }

                return Ok(result);
            }
            catch (FluentValidation.ValidationException ex)
            {
                _logger.LogWarning("Validation failed: {Errors}", ex.Message);
                return BadRequest(new
                {
                    Status = "error",
                    Message = "Validation failed",
                    Errors = ex.Errors.Select(e => e.ErrorMessage)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password");
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Status = "error",
                    Message = "An internal server error occurred",
                    Detail = ex.Message
                });
            }
        }
        [HttpPost("signin")]
        [ProducesResponseType(typeof(ApiResponse<SigninResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Signin([FromBody] SigninRequest request)
        {
            try
            {
                var command = new SigninCommand(request);
                var result = await _mediator.Send(command);

                if (result.Status == "error")
                {
                    return BadRequest(new
                    {
                        result.Status,
                        result.Message,
                        result.Errors
                    });
                }

                // Set the token in the cookie (optional)
                Response.Cookies.Append("access_token", result.Data.AccessToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = result.Data.TokenExpiry
                });

                return Ok(result);
            }
            catch (FluentValidation.ValidationException ex)
            {
                _logger.LogWarning("Validation failed: {Errors}", ex.Message);
                return BadRequest(new
                {
                    Status = "error",
                    Message = "Validation failed",
                    Errors = ex.Errors.Select(e => e.ErrorMessage)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user signin");
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Status = "error",
                    Message = "An internal server error occurred",
                    Detail = ex.Message
                });
            }
        }

        [HttpPost("refresh")]
        [ProducesResponseType(typeof(ApiResponse<TokenResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
        {
            try
            {
                // TODO: Implement refresh token logic
                return Ok(new
                {
                    Status = "success",
                    Message = "Refresh token endpoint",
                    Data = new { }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing token");
                return BadRequest(new
                {
                    Status = "error",
                    Message = "Invalid or expired token"
                });
            }
        }

        [Authorize]
        [HttpGet("profile")]
        [ProducesResponseType(typeof(ApiResponse<UserProfileResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                // Show all claims for debugging
                _logger.LogInformation("User authenticated: {IsAuthenticated}", User.Identity?.IsAuthenticated);
                _logger.LogInformation("User name: {Name}", User.Identity?.Name);

                foreach (var claim in User.Claims)
                {
                    _logger.LogInformation("Claim: {Type} = {Value}", claim.Type, claim.Value);
                }

                // Searching for the UserId in different ways
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                               ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                               ?? User.FindFirst("sub")?.Value
                               ?? User.FindFirst("userid")?.Value
                               ?? User.FindFirst("userId")?.Value;

                _logger.LogInformation("Found userId: {UserId}", userIdClaim);

                if (string.IsNullOrEmpty(userIdClaim))
                {
                    _logger.LogWarning("User ID not found in claims");
                    return BadRequest(new
                    {
                        Status = "error",
                        Message = "User ID not found in token"
                    });
                }

                if (!int.TryParse(userIdClaim, out int userId))
                {
                    _logger.LogWarning("Invalid user ID format: {UserId}", userIdClaim);
                    return BadRequest(new
                    {
                        Status = "error",
                        Message = "Invalid user ID format"
                    });
                }

                var query = new GetUserProfileQuery(userId);
                var result = await _mediator.Send(query);

                if (result.Status == "error")
                {
                    return NotFound(new
                    {
                        result.Status,
                        result.Message
                    });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user profile");
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Status = "error",
                    Message = "An internal server error occurred"
                });
            }
        }

        [Authorize]
        [HttpPost("signout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Signout()
        {
            // حذف الـ cookies
            Response.Cookies.Delete("access_token");
            Response.Cookies.Delete("refresh_token");

            return Ok(new
            {
                Status = "success",
                Message = "Logged out successfully"
            });
        }
        
        [HttpPost("signup")]
        [ProducesResponseType(typeof(ApiResponse<UserSignupResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Signup([FromBody] UserSignupRequest request)
        {
            try
            {
                var command = new SignupCommand(request);
                var result = await _mediator.Send(command);

                if (result.Status == "error")
                {
                    return Conflict(new
                    {
                        result.Status,
                        result.Message,
                        result.Errors
                    });
                }

                return CreatedAtAction(nameof(Signup), new { id = result.Data.UserId }, result);
            }
            catch (FluentValidation.ValidationException ex)
            {
                _logger.LogWarning("Validation failed: {Errors}", ex.Message);
                return BadRequest(new
                {
                    Status = "error",
                    Message = "Validation failed",
                    Errors = ex.Errors.Select(e => e.ErrorMessage)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user signup");
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Status = "error",
                    Message = "An internal server error occurred",
                    Detail = ex.Message
                });
            }
        }

        [HttpGet("check-availability")]
        [ProducesResponseType(typeof(ApiResponse<CheckAvailabilityResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CheckAvailability(
            [FromQuery] string email = null,
            [FromQuery] string username = null)
        {
            try
            {
                var query = new CheckAvailabilityQuery(new CheckAvailabilityRequest
                {
                    Email = email,
                    Username = username
                });

                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (FluentValidation.ValidationException ex)
            {
                _logger.LogWarning("Validation failed: {Errors}", ex.Message);
                return BadRequest(new
                {
                    Status = "error",
                    Message = "Validation failed",
                    Errors = ex.Errors.Select(e => e.ErrorMessage)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking availability");
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Status = "error",
                    Message = "An internal server error occurred"
                });
            }
        }
        
        [Authorize(Roles = "SuperAdmin,Admin,Manager")]
        [HttpPost("change-role")]
        [ProducesResponseType(typeof(ApiResponse<ChangeUserRoleResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> ChangeUserRole([FromBody] ChangeUserRoleRequest request)
        {
            try
            {
                // Obtain the current user ID from the token
                var currentUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    ?? User.FindFirst("sub")?.Value;

                if (string.IsNullOrEmpty(currentUserIdClaim) || !int.TryParse(currentUserIdClaim, out int currentUserId))
                {
                    return BadRequest(new
                    {
                        Status = "error",
                        Message = "Invalid authentication token"
                    });
                }

                var command = new ChangeUserRoleCommand(request, currentUserId);
                var result = await _mediator.Send(command);

                if (result.Status == "error")
                {
                    return BadRequest(new
                    {
                        result.Status,
                        result.Message,
                        result.Errors
                    });
                }

                return Ok(result);
            }
            catch (FluentValidation.ValidationException ex)
            {
                _logger.LogWarning("Validation failed: {Errors}", ex.Message);
                return BadRequest(new
                {
                    Status = "error",
                    Message = "Validation failed",
                    Errors = ex.Errors.Select(e => e.ErrorMessage)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing user role for UserId={UserId}", request.UserId);
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Status = "error",
                    Message = "An internal server error occurred",
                    Detail = ex.Message
                });
            }
        }

        [Authorize(Roles = "SuperAdmin,Admin,Manager")]
        [HttpGet("{userId}/role-history")]
        [ProducesResponseType(typeof(ApiResponse<List<UserRoleHistoryResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetUserRoleHistory(
            int userId,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            try
            {
                var query = new GetUserRoleHistoryQuery(userId, fromDate, toDate);
                var result = await _mediator.Send(query);

                if (result.Status == "error")
                {
                    return NotFound(new
                    {
                        result.Status,
                        result.Message
                    });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving role history for UserId={UserId}", userId);
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Status = "error",
                    Message = "An internal server error occurred"
                });
            }
        }

        [Authorize(Roles = "SuperAdmin,Admin,Manager")]
        [HttpPost("{userId}/assign-role")]
        [ProducesResponseType(typeof(ApiResponse<ChangeUserRoleResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> AssignRole(
            int userId,
            [FromBody] AssignRoleRequest request)
        {
            try
            {
                // 
                var changeRoleRequest = new ChangeUserRoleRequest
                {
                    UserId = userId,
                    NewRoleId = request.RoleId,
                    Reason = request.Reason
                };

                var currentUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    ?? User.FindFirst("sub")?.Value;

                if (string.IsNullOrEmpty(currentUserIdClaim) || !int.TryParse(currentUserIdClaim, out int currentUserId))
                {
                    return BadRequest(new
                    {
                        Status = "error",
                        Message = "Invalid authentication token"
                    });
                }

                var command = new ChangeUserRoleCommand(changeRoleRequest, currentUserId);
                var result = await _mediator.Send(command);

                if (result.Status == "error")
                {
                    return BadRequest(new
                    {
                        result.Status,
                        result.Message,
                        result.Errors
                    });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning role to UserId={UserId}", userId);
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Status = "error",
                    Message = "An internal server error occurred"
                });
            }
        }

        // DTO 
        public class AssignRoleRequest
        {
            public int RoleId { get; set; }
            public string Reason { get; set; }
        }
    }
}
