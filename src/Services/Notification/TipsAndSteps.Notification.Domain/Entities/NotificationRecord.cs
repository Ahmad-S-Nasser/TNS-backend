namespace TipsAndSteps.Notification.Domain.Entities;

public sealed class NotificationRecord
{
    public string  Id          { get; set; } = Guid.NewGuid().ToString();
    public string  UserId      { get; set; } = string.Empty;
    public string  Title       { get; set; } = string.Empty;
    public string  Body        { get; set; } = string.Empty;
    public string  Type        { get; set; } = "Push"; // Push | SMS | Email
    public bool    IsRead      { get; set; }
    public bool    IsSent      { get; set; }
    public string? FcmMessageId { get; set; }
    public DateTime SentAt     { get; set; } = DateTime.UtcNow;
}
