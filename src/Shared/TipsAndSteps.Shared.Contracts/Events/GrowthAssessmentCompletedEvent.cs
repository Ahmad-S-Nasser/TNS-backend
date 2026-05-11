namespace TipsAndSteps.Shared.Contracts.Events;

public sealed record GrowthAssessmentCompletedEvent(
    string AssessmentId,
    string ChildId,
    string ParentId,
    string AgeGroup,       // 0-3months | 4-6months | 7-12months | 1-2years | 2-3years
    string Category,       // Cognitive | Motor | Language | Social | SelfCare
    decimal TotalScore,
    string ScoreLevel,     // Excellent | Good | NeedsAttention | RequiresIntervention
    DateTime CompletedAt);
