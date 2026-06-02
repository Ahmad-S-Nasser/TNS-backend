using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TipsAndSteps.UserManagement.Application.Commands.RegisterUser;
using TipsAndSteps.UserManagement.Application.Commands.UpdateProfile;
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
                     ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                     ?? throw new UnauthorizedAccessException("No sub or NameIdentifier claim in token.");
        var result = await _mediator.Send(new GetUserQuery(userId), ct);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>Update the current authenticated user's own profile</summary>
    [HttpPut("me")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateMe(
        [FromBody] UpdateProfileRequest request,
        CancellationToken ct)
    {
        var userId = User.FindFirst("sub")?.Value
                     ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                     ?? throw new UnauthorizedAccessException("No sub or NameIdentifier claim in token.");
        var command = new UpdateProfileCommand(
            userId,
            request.FirstName,
            request.LastName,
            request.PhoneNumber,
            request.PreferredLanguage);
        var result = await _mediator.Send(command, ct);
        return result ? NoContent() : NotFound();
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

/// <summary>Request body for PUT /api/users/me</summary>
public sealed record UpdateProfileRequest(
    string? FirstName,
    string? LastName,
    string? PhoneNumber,
    string? PreferredLanguage);
