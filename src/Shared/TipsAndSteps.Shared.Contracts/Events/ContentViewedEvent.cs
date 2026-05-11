namespace TipsAndSteps.Shared.Contracts.Events;

public sealed record ContentViewedEvent(
    string ContentId,
    string ContentType,   // Article | Video | Infographic
    string ContentSection,// BehavioralProblems | HealthyNutrition | Vaccines | ...
    string UserId,
    string? ChildId,
    DateTime ViewedAt);
