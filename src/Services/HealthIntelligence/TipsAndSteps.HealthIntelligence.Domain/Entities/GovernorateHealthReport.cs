namespace TipsAndSteps.HealthIntelligence.Domain.Entities;

/// <summary>
/// Egypt national health heatmap data point.
/// k-anonymity applied: only published if AggregateCount >= 5 (PDPL compliance).
/// </summary>
public sealed class GovernorateHealthReport
{
    public string   Id               { get; set; } = Guid.NewGuid().ToString();
    public string   GovernorateCode  { get; set; } = string.Empty; // EG-XX
    public string   GovernorateNameAr { get; set; } = string.Empty;
    public string   GovernorateNameEn { get; set; } = string.Empty;
    public string   HealthIndicator  { get; set; } = string.Empty; // Vaccination | GrowthDelay | NutritionRisk
    public int      AggregateCount   { get; set; }  // k-anonymity: must be >= 5
    public decimal  Prevalence       { get; set; }  // percentage
    public DateTime ReportPeriod     { get; set; }
    public DateTime GeneratedAt      { get; set; } = DateTime.UtcNow;
}
