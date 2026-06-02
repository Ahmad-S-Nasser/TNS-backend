using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TipsAndSteps.UserManagement.Application.Commands.CreateChild;
using TipsAndSteps.UserManagement.Application.Queries.GetChildren;

namespace TipsAndSteps.UserManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "parent,Parent")]
public sealed class ChildrenController(ISender sender) : ControllerBase
{
    /// <summary>Get list of children profiles for the current authenticated parent</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ChildDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyChildren(CancellationToken ct)
    {
        var parentId = User.FindFirst("sub")?.Value
                     ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                     ?? throw new UnauthorizedAccessException("No sub or NameIdentifier claim in token.");

        var children = await sender.Send(new GetChildrenQuery(parentId), ct);
        return Ok(children);
    }

    /// <summary>Create a new child profile for the current authenticated parent</summary>
    [HttpPost]
    [ProducesResponseType(typeof(CreateChildResult), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateChild([FromBody] CreateChildRequest request, CancellationToken ct)
    {
        var parentId = User.FindFirst("sub")?.Value
                     ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                     ?? throw new UnauthorizedAccessException("No sub or NameIdentifier claim in token.");

        var command = new CreateChildCommand(
            parentId,
            request.FullName,
            request.DateOfBirth,
            request.Gender,
            request.BloodType);

        var result = await sender.Send(command, ct);
        return Created($"/api/children/{result.Id}", result);
    }
}

public sealed record CreateChildRequest(
    string FullName,
    DateTime DateOfBirth,
    string Gender,
    string? BloodType);
