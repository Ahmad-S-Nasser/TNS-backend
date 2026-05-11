using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TipsAndSteps.UserManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "parent")]
public sealed class ChildrenController : ControllerBase
{
    // TODO: Implement child profile CRUD using CQRS (CreateChildCommand, etc.)
    [HttpGet]
    public IActionResult GetMyChildren()
        => Ok(new { message = "Children endpoint — implement via CQRS commands/queries" });
}
