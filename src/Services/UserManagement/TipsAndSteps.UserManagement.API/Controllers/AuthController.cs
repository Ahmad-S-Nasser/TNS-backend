using MediatR;
using Microsoft.AspNetCore.Mvc;
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
        {
            return Unauthorized(new { message = result.Message });
        }

        return Ok(result);
    }
}
