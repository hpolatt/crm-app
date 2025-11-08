using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CrmApp.Core.DTOs;
using CrmApp.Core.DTOs.Users;
using CrmApp.Core.Interfaces;
using CrmApp.Application.Users.Queries;
using CrmApp.Application.Users.Commands;

namespace CrmApp.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UsersController : BaseController
{
    private readonly IMediator _mediator;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IMediator mediator, ILogger<UsersController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<ActionResult<ApiResponse<List<UserDto>>>> GetAll()
    {
        try
        {
            var query = new GetAllUsersQuery();
            var users = await _mediator.Send(query);
            return Ok(ApiResponse<List<UserDto>>.SuccessResponse(users));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all users");
            return StatusCode(500, ApiResponse<List<UserDto>>.ErrorResponse("An error occurred while retrieving users"));
        }
    }

    [HttpGet("basic")]
    [Authorize] // Tüm authenticated kullanıcılar erişebilir
    public async Task<ActionResult<ApiResponse<List<object>>>> GetBasicList()
    {
        try
        {
            var query = new GetAllUsersQuery();
            var users = await _mediator.Send(query);
            
            // Sadece id, email, firstName, lastName döndür
            var basicUsers = users.Select(u => new
            {
                u.Id,
                u.Email,
                u.FirstName,
                u.LastName,
                FullName = $"{u.FirstName} {u.LastName}".Trim()
            }).ToList();
            
            return Ok(ApiResponse<List<object>>.SuccessResponse(
                basicUsers.Cast<object>().ToList(),
                "Basic user list retrieved successfully"
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting basic user list");
            return StatusCode(500, ApiResponse<List<object>>.ErrorResponse("An error occurred while retrieving users"));
        }
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<ActionResult<ApiResponse<UserDto>>> GetById(Guid id)
    {
        try
        {
            var query = new GetUserByIdQuery { Id = id };
            var user = await _mediator.Send(query);

            if (user == null)
            {
                return NotFound(ApiResponse<UserDto>.ErrorResponse("User not found"));
            }

            return Ok(ApiResponse<UserDto>.SuccessResponse(user));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user {UserId}", id);
            return StatusCode(500, ApiResponse<UserDto>.ErrorResponse("An error occurred while retrieving the user"));
        }
    }

    [HttpPost]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<ActionResult<ApiResponse<UserDto>>> Create([FromBody] CreateUserRequest request)
    {
        try
        {
            var command = new CreateUserCommand { Request = request };
            var user = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, ApiResponse<UserDto>.SuccessResponse(user, "User created successfully"));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Failed to create user");
            return BadRequest(ApiResponse<UserDto>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user");
            return StatusCode(500, ApiResponse<UserDto>.ErrorResponse("An error occurred while creating the user"));
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<ActionResult<ApiResponse<UserDto>>> Update(Guid id, [FromBody] UpdateUserRequest request)
    {
        try
        {
            var command = new UpdateUserCommand { Id = id, Request = request };
            var user = await _mediator.Send(command);
            return Ok(ApiResponse<UserDto>.SuccessResponse(user, "User updated successfully"));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Failed to update user {UserId}", id);
            return NotFound(ApiResponse<UserDto>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user {UserId}", id);
            return StatusCode(500, ApiResponse<UserDto>.ErrorResponse("An error occurred while updating the user"));
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
    {
        try
        {
            var command = new DeleteUserCommand { Id = id };
            var result = await _mediator.Send(command);

            if (!result)
            {
                return NotFound(ApiResponse<bool>.ErrorResponse("User not found"));
            }

            return Ok(ApiResponse<bool>.SuccessResponse(true, "User deleted successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user {UserId}", id);
            return StatusCode(500, ApiResponse<bool>.ErrorResponse("An error occurred while deleting the user"));
        }
    }

    [HttpPost("{id}/activate")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<ActionResult<ApiResponse<bool>>> Activate(Guid id, [FromServices] IAuthService authService)
    {
        try
        {
            var result = await authService.ActivateUserAsync(id);
            return Ok(ApiResponse<bool>.SuccessResponse(result, "User activated successfully"));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Failed to activate user {UserId}", id);
            return NotFound(ApiResponse<bool>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error activating user {UserId}", id);
            return StatusCode(500, ApiResponse<bool>.ErrorResponse("An error occurred while activating the user"));
        }
    }

    [HttpPost("{id}/deactivate")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<ActionResult<ApiResponse<bool>>> Deactivate(Guid id, [FromServices] IAuthService authService)
    {
        try
        {
            var result = await authService.DeactivateUserAsync(id);
            return Ok(ApiResponse<bool>.SuccessResponse(result, "User deactivated successfully"));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Failed to deactivate user {UserId}", id);
            return NotFound(ApiResponse<bool>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deactivating user {UserId}", id);
            return StatusCode(500, ApiResponse<bool>.ErrorResponse("An error occurred while deactivating the user"));
        }
    }
}
