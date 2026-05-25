using TipsAndSteps.GrowthMatrix.Domain.Enums;

namespace TipsAndSteps.GrowthMatrix.Domain.ValueObjects;

public sealed record ScoringResult(
    decimal TotalScore,           // 0-100
    ScoreLevel Level,
    Dictionary<string, decimal> CategoryScores,
    List<string> RecommendedActions);
