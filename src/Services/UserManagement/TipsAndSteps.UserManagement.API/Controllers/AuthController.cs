using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TipsAndSteps.UserManagement.Application.Commands.ChangePassword;
using TipsAndSteps.UserManagement.Application.Commands.Login;

namespace TipsAndSteps.UserManagement.API.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(ISender sender) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var result = await sender.Send(command);

        if (!result.Success)
            return Unauthorized(new { message = result.Message });

        return Ok(result);
    }

    /// <summary>Change the current authenticated user's password</summary>
    [HttpPost("change-password")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordRequest request,
        CancellationToken ct)
    {
        var userId = User.FindFirst("sub")?.Value
                     ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                     ?? throw new UnauthorizedAccessException("No sub or NameIdentifier claim in token.");

        var result = await sender.Send(
            new ChangePasswordCommand(userId, request.CurrentPassword, request.NewPassword), ct);

        if (!result.Success)
            return BadRequest(new { message = result.Message });

        return NoContent();
    }
}

/// <summary>Request body for POST /api/auth/change-password</summary>
public sealed record ChangePasswordRequest(string CurrentPassword, string NewPassword);
