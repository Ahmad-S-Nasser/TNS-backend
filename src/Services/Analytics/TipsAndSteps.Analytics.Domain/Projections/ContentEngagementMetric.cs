namespace TipsAndSteps.Analytics.Domain.Projections;

public sealed class ContentEngagementMetric
{
    public string   Id          { get; set; } = Guid.NewGuid().ToString();
    public string   ContentId   { get; set; } = string.Empty;
    public string   Section     { get; set; } = string.Empty;
    public int      Views       { get; set; }
    public int      UniqueUsers { get; set; }
    public double   AvgRating   { get; set; }
    public DateTime Date        { get; set; }
}
