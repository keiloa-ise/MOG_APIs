using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MOJ.Modules.UserManagments.Application.Features.Departments.Commands.CreateDepartment;
using MOJ.Modules.UserManagments.Application.Features.Departments.DTOs;
using MOJ.Modules.UserManagments.Application.Features.Departments.Queries.GetAllDepartments;
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
    [Route("api/departments")]
    [ApiController]
    public class DepartmentsController : ApiControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<DepartmentsController> _logger;

        public DepartmentsController(IMediator mediator, ILogger<DepartmentsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<DepartmentDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAllDepartments(
            [FromQuery] bool? isActive = null,
            [FromQuery] bool includeHierarchy = false)
        {
            try
            {
                var query = new GetAllDepartmentsQuery(isActive, includeHierarchy);
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving departments");
                return StatusCode(500, new { Status = "error", Message = "Internal server error" });
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<DepartmentDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateDepartment([FromBody] CreateDepartmentRequest request)
        {
            try
            {
                var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var command = new CreateDepartmentCommand(request, currentUserId);
                var result = await _mediator.Send(command);

                if (result.Status == "success")
                    return CreatedAtAction(nameof(GetAllDepartments), new { id = result.Data.Id }, result);

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating department");
                return StatusCode(500, new { Status = "error", Message = "Internal server error" });
            }
        }
    }
}
