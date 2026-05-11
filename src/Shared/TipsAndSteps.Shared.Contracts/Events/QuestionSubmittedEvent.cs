namespace TipsAndSteps.Shared.Contracts.Events;

public sealed record QuestionSubmittedEvent(
    string QuestionId,
    string ParentId,
    string? DoctorId,
    string Category,
    string QuestionText,
    bool IsAnonymous,
    DateTime SubmittedAt);
