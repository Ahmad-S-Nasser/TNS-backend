using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TipsAndSteps.Notification.Infrastructure.Persistence;

namespace TipsAndSteps.Notification.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly INotificationRepository _repo;

    public NotificationsController(INotificationRepository repo) => _repo = repo;

    [HttpGet]
    public async Task<IActionResult> GetNotifications()
    {
        var notifications = await _repo.GetAdminNotificationsAsync();
        return Ok(notifications);
    }
}
