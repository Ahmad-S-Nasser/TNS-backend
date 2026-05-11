using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TipsAndSteps.UserManagement.Application.Commands.UpdateRoleDefaults;
using TipsAndSteps.UserManagement.Application.Queries.GetRoleDefaults;

namespace TipsAndSteps.UserManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class RBACController : ControllerBase
{
    private readonly IMediator _mediator;

    public RBACController(IMediator mediator) => _mediator = mediator;

    [HttpGet("defaults")]
    public async Task<IActionResult> GetDefaults(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetRoleDefaultsQuery(), ct);
        return Ok(result);
    }

    [HttpPatch("defaults/{category}")]
    public async Task<IActionResult> UpdateDefaults(string category, [FromBody] List<string> permissions, CancellationToken ct)
    {
        var result = await _mediator.Send(new UpdateRoleDefaultsCommand(category, permissions), ct);
        return result ? NoContent() : NotFound();
    }
}
