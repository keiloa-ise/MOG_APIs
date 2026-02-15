using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MOJ.Modules.UserManagments.Application.Features.Users.Commands.AssignDepartments;
using MOJ.Modules.UserManagments.Application.Features.Users.Commands.UpdateUserDepartments;
using MOJ.Modules.UserManagments.Application.Features.Users.DTOs;
using MOJ.Modules.UserManagments.Application.Features.Users.Queries.GetUserDepartments;
using MOJ.Shared.Application;
using MOJ.Shared.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MOJ.Modules.UserManagments.API.Controllers
{

    [Authorize(Roles = "SuperAdmin,Admin")]
    [Route("api/users/{userId}/departments")]
    [ApiController]
    public class UserDepartmentsController : ApiControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<UserDepartmentsController> _logger;

        public UserDepartmentsController(IMediator mediator, ILogger<UserDepartmentsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// الحصول على جميع أقسام مستخدم معين
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<UserDepartmentDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserDepartments(int userId)
        {
            try
            {
                var query = new GetUserDepartmentsQuery(userId);
                var result = await _mediator.Send(query);

                if (result.Status == "error" && result.Message.Contains("not found"))
                    return NotFound(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving departments for user {UserId}", userId);
                return StatusCode(500, new
                {
                    Status = "error",
                    Message = "An internal server error occurred while retrieving user departments"
                });
            }
        }

        /// <summary>
        /// تعيين أقسام جديدة لمستخدم (استبدال الأقسام الحالية)
        /// </summary>
        [HttpPost("assign")]
        [ProducesResponseType(typeof(ApiResponse<List<UserDepartmentDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AssignDepartments(
            int userId,
            [FromBody] AssignDepartmentsRequest request)
        {
            try
            {
                // التحقق من صحة الطلب
                if (request.DepartmentIds == null || !request.DepartmentIds.Any())
                {
                    return BadRequest(new
                    {
                        Status = "error",
                        Message = "At least one department must be selected"
                    });
                }

                request.UserId = userId;

                var currentUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(currentUserIdClaim) || !int.TryParse(currentUserIdClaim, out int currentUserId))
                {
                    return BadRequest(new
                    {
                        Status = "error",
                        Message = "Invalid authentication token"
                    });
                }

                var command = new AssignDepartmentsCommand(request, currentUserId);
                var result = await _mediator.Send(command);

                if (result.Status == "success")
                    return Ok(result);

                if (result.Message.Contains("not found"))
                    return NotFound(result);

                return BadRequest(result);
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
                _logger.LogError(ex, "Error assigning departments to user {UserId}", userId);
                return StatusCode(500, new
                {
                    Status = "error",
                    Message = "An internal server error occurred while assigning departments"
                });
            }
        }

        /// <summary>
        /// تحديث أقسام المستخدم (إضافة وحذف أقسام محددة)
        /// </summary>
        [HttpPut("update")]
        [ProducesResponseType(typeof(ApiResponse<List<UserDepartmentDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateUserDepartments(
            int userId,
            [FromBody] UpdateUserDepartmentsRequest request)
        {
            try
            {
                request.UserId = userId;

                var currentUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(currentUserIdClaim) || !int.TryParse(currentUserIdClaim, out int currentUserId))
                {
                    return BadRequest(new
                    {
                        Status = "error",
                        Message = "Invalid authentication token"
                    });
                }

                var command = new UpdateUserDepartmentsCommand(request, currentUserId);
                var result = await _mediator.Send(command);

                if (result.Status == "success")
                    return Ok(result);

                if (result.Message.Contains("not found"))
                    return NotFound(result);

                return BadRequest(result);
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
                _logger.LogError(ex, "Error updating departments for user {UserId}", userId);
                return StatusCode(500, new
                {
                    Status = "error",
                    Message = "An internal server error occurred while updating departments"
                });
            }
        }

        /// <summary>
        /// حذف جميع أقسام المستخدم
        /// </summary>
        [HttpDelete("clear")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ClearUserDepartments(int userId)
        {
            try
            {
                var currentUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(currentUserIdClaim) || !int.TryParse(currentUserIdClaim, out int currentUserId))
                {
                    return BadRequest(new
                    {
                        Status = "error",
                        Message = "Invalid authentication token"
                    });
                }

                // إنشاء طلب فارغ لإزالة جميع الأقسام
                var request = new UpdateUserDepartmentsRequest
                {
                    UserId = userId,
                    AddDepartmentIds = new List<int>(),
                    RemoveDepartmentIds = (await _mediator.Send(new GetUserDepartmentsQuery(userId)))
                        ?.Data?.Select(d => d.DepartmentId).ToList() ?? new List<int>()
                };

                var command = new UpdateUserDepartmentsCommand(request, currentUserId);
                var result = await _mediator.Send(command);

                if (result.Status == "success")
                {
                    return Ok(new
                    {
                        Status = "success",
                        Message = "All departments removed successfully"
                    });
                }

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing departments for user {UserId}", userId);
                return StatusCode(500, new
                {
                    Status = "error",
                    Message = "An internal server error occurred while clearing departments"
                });
            }
        }

        /// <summary>
        /// تعيين قسم أساسي للمستخدم
        /// </summary>
        [HttpPatch("set-primary/{departmentId}")]
        [ProducesResponseType(typeof(ApiResponse<List<UserDepartmentDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SetPrimaryDepartment(int userId, int departmentId)
        {
            try
            {
                var currentUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(currentUserIdClaim) || !int.TryParse(currentUserIdClaim, out int currentUserId))
                {
                    return BadRequest(new
                    {
                        Status = "error",
                        Message = "Invalid authentication token"
                    });
                }

                // التحقق من أن المستخدم لديه هذا القسم
                var userDepartments = await _mediator.Send(new GetUserDepartmentsQuery(userId));
                if (!userDepartments.Data?.Any(d => d.DepartmentId == departmentId) ?? true)
                {
                    return BadRequest(new
                    {
                        Status = "error",
                        Message = "User does not have this department"
                    });
                }

                var request = new UpdateUserDepartmentsRequest
                {
                    UserId = userId,
                    PrimaryDepartmentId = departmentId
                };

                var command = new UpdateUserDepartmentsCommand(request, currentUserId);
                var result = await _mediator.Send(command);

                if (result.Status == "success")
                    return Ok(result);

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting primary department for user {UserId}", userId);
                return StatusCode(500, new
                {
                    Status = "error",
                    Message = "An internal server error occurred while setting primary department"
                });
            }
        }

        /// <summary>
        /// التحقق من وجود مستخدم في قسم معين
        /// </summary>
        [HttpGet("check/{departmentId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CheckUserInDepartment(int userId, int departmentId)
        {
            try
            {
                var userDepartments = await _mediator.Send(new GetUserDepartmentsQuery(userId));
                var isInDepartment = userDepartments.Data?.Any(d => d.DepartmentId == departmentId) ?? false;

                return Ok(new
                {
                    Status = "success",
                    Data = new
                    {
                        UserId = userId,
                        DepartmentId = departmentId,
                        IsInDepartment = isInDepartment,
                        IsPrimary = userDepartments.Data?.FirstOrDefault(d => d.DepartmentId == departmentId)?.IsPrimary ?? false
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking user {UserId} in department {DepartmentId}", userId, departmentId);
                return StatusCode(500, new
                {
                    Status = "error",
                    Message = "An internal server error occurred while checking department membership"
                });
            }
        }

        /// <summary>
        /// الحصول على إحصائيات أقسام المستخدم
        /// </summary>
        [HttpGet("stats")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetUserDepartmentsStats(int userId)
        {
            try
            {
                var userDepartments = await _mediator.Send(new GetUserDepartmentsQuery(userId));
                var departments = userDepartments.Data ?? new List<UserDepartmentDto>();

                var stats = new
                {
                    TotalDepartments = departments.Count,
                    PrimaryDepartment = departments.FirstOrDefault(d => d.IsPrimary),
                    DepartmentIds = departments.Select(d => d.DepartmentId).ToList(),
                    DepartmentNames = departments.Select(d => d.DepartmentName).ToList(),
                    AssignedAt = departments.Select(d => d.AssignedAt).ToList()
                };

                return Ok(new
                {
                    Status = "success",
                    Data = stats
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving departments stats for user {UserId}", userId);
                return StatusCode(500, new
                {
                    Status = "error",
                    Message = "An internal server error occurred while retrieving departments stats"
                });
            }
        }
    }
}
