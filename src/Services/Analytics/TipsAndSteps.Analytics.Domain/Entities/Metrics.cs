namespace TipsAndSteps.Analytics.Domain.Entities;

public sealed class MonthlyMetric 
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string YearMonth { get; set; } = string.Empty; // e.g. "2024-01"
    public string MonthName { get; set; } = string.Empty; // e.g. "Jan"
    public long Users { get; set; }
    public long Content { get; set; }
    public double Engagement { get; set; }
}

public sealed class ContentEngagementMetric
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Type { get; set; } = string.Empty; // Tips, Articles, etc.
    public long Views { get; set; }
    public long Likes { get; set; }
    public long Shares { get; set; }
}
