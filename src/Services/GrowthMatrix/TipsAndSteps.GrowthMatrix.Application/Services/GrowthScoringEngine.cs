using TipsAndSteps.GrowthMatrix.Domain.Entities;
using TipsAndSteps.GrowthMatrix.Domain.Enums;

namespace TipsAndSteps.GrowthMatrix.Application.Services;

/// <summary>
/// Core scoring engine for Growth Matrix assessments.
/// Calculates weighted scores per category and overall developmental level.
/// </summary>
public sealed class GrowthScoringEngine
{
    public ScoringResult Calculate(GrowthAssessment assessment, IReadOnlyList<GrowthSkill> skills)
    {
        var categoryScores  = new Dictionary<string, decimal>();
        var skillsById      = skills.ToDictionary(s => s.Id);

        // Group responses by category
        var byCategory = assessment.Responses
            .Where(r => skillsById.ContainsKey(r.SkillId))
            .GroupBy(r => skillsById[r.SkillId].CategoryId);

        foreach (var group in byCategory)
        {
            decimal earned  = 0m;
            decimal maxPossible = 0m;

            foreach (var response in group)
            {
                var skill = skillsById[response.SkillId];
                
                // Weight contributes to max possible
                maxPossible += (decimal)skill.Weight;

                earned += skill.MetricType switch
                {
                    MetricType.Boolean => response.YesNoValue == true ? (decimal)skill.Weight : 0m,
                    MetricType.Numeric => CalculateNumericScore(response, skill),
                    MetricType.Scale => CalculateScaleScore(response, skill),
                    _ => 0m
                };
            }

            var pct = maxPossible > 0 ? (earned / maxPossible) * 100m : 0m;
            categoryScores[group.Key] = Math.Round(pct, 1);
        }

        var totalScore = categoryScores.Any()
            ? Math.Round(categoryScores.Values.Average(), 1)
            : 0m;

        var level = totalScore switch
        {
            >= 90m => ScoreLevel.Excellent,
            >= 70m => ScoreLevel.Good,
            >= 50m => ScoreLevel.NeedsAttention,
            _      => ScoreLevel.RequiresIntervention
        };

        var recommendations = BuildRecommendations(level, categoryScores);

        return new ScoringResult(totalScore, level, categoryScores, recommendations);
    }

    private decimal CalculateNumericScore(SkillResponse response, GrowthSkill skill)
    {
        // Simple logic for now: if value > 0, it's partially achieved
        // Ideally this should compare against GrowthRules
        return (response.NumericValue ?? 0m) > 0 ? (decimal)skill.Weight : 0m;
    }

    private decimal CalculateScaleScore(SkillResponse response, GrowthSkill skill)
    {
        // For scale, we might use the numeric value as a percentage of a max (e.g. 4)
        var val = (decimal)(response.NumericValue ?? 0m);
        return (val / 4m) * (decimal)skill.Weight;
    }

    private static List<string> BuildRecommendations(
        ScoreLevel level,
        Dictionary<string, decimal> categoryScores)
    {
        var recs = new List<string>();

        if (level == ScoreLevel.RequiresIntervention)
            recs.Add("يُنصح بمراجعة طبيب متخصص في تطور الأطفال");

        if (level == ScoreLevel.NeedsAttention)
            recs.Add("تابع تطور طفلك وراجع قسم الأنشطة المقترحة");

        foreach (var (categoryId, score) in categoryScores.Where(c => c.Value < 60))
            recs.Add($"يحتاج تحسين في مجال معين — تحقق من الأنشطة المقترحة");

        return recs;
    }
}

public sealed record ScoringResult(
    decimal TotalScore, 
    ScoreLevel Level, 
    Dictionary<string, decimal> CategoryScores, 
    List<string> RecommendedActions);
