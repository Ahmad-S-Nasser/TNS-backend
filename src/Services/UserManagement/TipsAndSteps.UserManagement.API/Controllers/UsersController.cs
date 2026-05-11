using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TipsAndSteps.UserManagement.Application.Commands.RegisterUser;
using TipsAndSteps.UserManagement.Application.Queries.GetUser;
using TipsAndSteps.UserManagement.Domain.Enums;

namespace TipsAndSteps.UserManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator) => _mediator = mediator;

    /// <summary>Register a new user (parent or doctor)</summary>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(RegisterUserResult), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(
        [FromBody] RegisterUserCommand command,
        CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.UserId }, result);
    }

    /// <summary>Get user profile by ID</summary>
    [HttpGet("{id}")]
    [Authorize]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(string id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetUserQuery(id), ct);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>Get current authenticated user's profile</summary>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMe(CancellationToken ct)
    {
        var userId = User.FindFirst("sub")?.Value
                     ?? throw new UnauthorizedAccessException("No sub claim in token.");
        var result = await _mediator.Send(new GetUserQuery(userId), ct);
        return result is null ? NotFound() : Ok(result);
    }
    /// <summary>List all users (paginated)</summary>
    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(IReadOnlyList<UserDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(new TipsAndSteps.UserManagement.Application.Queries.ListUsers.ListUsersQuery(page, pageSize), ct);
        return Ok(result);
    }

    /// <summary>Update user permission overrides</summary>
    [HttpPatch("{id}/permissions")]
    [Authorize]
    public async Task<IActionResult> UpdatePermissions(
        string id,
        [FromBody] List<TipsAndSteps.UserManagement.Application.Commands.UpdateUserPermissions.PermissionOverrideDto> overrides,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new TipsAndSteps.UserManagement.Application.Commands.UpdateUserPermissions.UpdateUserPermissionsCommand(id, overrides), ct);
        return result ? NoContent() : NotFound();
    }
}
