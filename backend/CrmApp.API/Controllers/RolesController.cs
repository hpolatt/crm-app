using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CrmApp.Core.DTOs;
using CrmApp.Core.DTOs.Roles;
using CrmApp.Application.Roles.Queries;
using CrmApp.Application.Roles.Commands;

namespace CrmApp.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class RolesController : BaseController
{
    private readonly IMediator _mediator;
    private readonly ILogger<RolesController> _logger;

    public RolesController(IMediator mediator, ILogger<RolesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<ActionResult<ApiResponse<List<RoleDto>>>> GetAll()
    {
        try
        {
            var query = new GetAllRolesQuery();
            var roles = await _mediator.Send(query);
            return Ok(ApiResponse<List<RoleDto>>.SuccessResponse(roles));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all roles");
            return StatusCode(500, ApiResponse<List<RoleDto>>.ErrorResponse("An error occurred while retrieving roles"));
        }
    }

    [HttpPost]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<ActionResult<ApiResponse<RoleDto>>> Create([FromBody] CreateRoleRequest request)
    {
        try
        {
            var command = new CreateRoleCommand { Request = request };
            var role = await _mediator.Send(command);
            return Ok(ApiResponse<RoleDto>.SuccessResponse(role, "Role created successfully"));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Failed to create role");
            return BadRequest(ApiResponse<RoleDto>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating role");
            return StatusCode(500, ApiResponse<RoleDto>.ErrorResponse("An error occurred while creating the role"));
        }
    }
}
