namespace TipsAndSteps.Shared.Contracts.Events;

public sealed record HealthDataReportedEvent(
    string ReportId,
    string GovernorateCode,  // Egypt governorate code
    string DistrictCode,
    string HealthIndicator,  // Vaccination | GrowthDelay | NutritionRisk
    int AggregateCount,      // k-anonymity: min 5
    DateTime ReportedAt);
