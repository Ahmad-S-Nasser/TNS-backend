namespace TipsAndSteps.Analytics.Domain.Entities;

public class AnalyticsKPI
{
    public string Id { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public long Value { get; set; }
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}
