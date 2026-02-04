using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MOJ.Modules.UserManagments.Application.Features.Roles.DTOs;
using MOJ.Modules.UserManagments.Application.Features.Roles.Queries.GetAllRoles;
using MOJ.Shared.Application;
using MOJ.Shared.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOJ.Modules.UserManagments.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "SuperAdmin,Admin")] 
    public class RolesController : ApiControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<RolesController> _logger;

        public RolesController(IMediator mediator, ILogger<RolesController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous] // السماح للجميع برؤية الأدوار المتاحة
        [ProducesResponseType(typeof(ApiResponse<List<RoleDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllRoles([FromQuery] bool? isActive = null)
        {
            try
            {
                var query = new GetAllRolesQuery(isActive);
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving roles");
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Status = "error",
                    Message = "An internal server error occurred"
                });
            }
        }

        [HttpGet("available")]
        [ProducesResponseType(typeof(ApiResponse<List<RoleDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAvailableRoles()
        {
            try
            {
                var query = new GetAllRolesQuery(true); 
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving available roles");
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Status = "error",
                    Message = "An internal server error occurred"
                });
            }
        }
    }
}
