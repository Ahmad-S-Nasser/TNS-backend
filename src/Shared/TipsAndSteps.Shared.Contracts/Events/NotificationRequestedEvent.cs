namespace TipsAndSteps.Shared.Contracts.Events;

public sealed record NotificationRequestedEvent(
    string NotificationId,
    string UserId,
    string Title,
    string Body,
    string Type,           // Push | SMS | Email
    Dictionary<string, string> Data,
    DateTime RequestedAt);
