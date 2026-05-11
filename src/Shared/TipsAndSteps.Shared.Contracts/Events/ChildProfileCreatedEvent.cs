namespace TipsAndSteps.Shared.Contracts.Events;

public sealed record ChildProfileCreatedEvent(
    string ChildId,
    string ParentId,
    string FullName,
    DateTime DateOfBirth,
    string Gender,         // Male | Female
    DateTime CreatedAt);
