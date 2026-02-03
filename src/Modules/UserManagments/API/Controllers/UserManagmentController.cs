using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MOJ.Modules.UserManagments.Application.Features.Users.Commands.Signin;
using MOJ.Modules.UserManagments.Application.Features.Users.Commands.Signup;
using MOJ.Modules.UserManagments.Application.Features.Users.DTOs;
using MOJ.Modules.UserManagments.Application.Features.Users.Queries.CheckAvailability;
using MOJ.Modules.UserManagments.Application.Features.Users.Queries.GetUserProfile;
using MOJ.Shared.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MOJ.Modules.UserManagments.API.Controllers
{
    [Route("api/roles")]
    public class UserManagmentController : ApiControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<UserManagmentController> _logger;
        public UserManagmentController(IMediator mediator, ILogger<UserManagmentController> logger)
        {
            _mediator = mediator;
            _logger = logger;
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
                var userIdClaim = User.FindFirst("sub")?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return BadRequest(new
                    {
                        Status = "error",
                        Message = "Invalid token"
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
    }
}
