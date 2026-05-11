namespace TipsAndSteps.Analytics.Domain.Projections;

/// <summary>Kafka-projected read model in tips-steps-analytics-read MongoDB</summary>
public sealed class DailyUserMetric
{
    public string   Id            { get; set; } = Guid.NewGuid().ToString();
    public DateTime Date          { get; set; }
    public int      NewRegistrations { get; set; }
    public int      ActiveUsers   { get; set; }
    public int      TotalAssessments { get; set; }
    public int      ContentViews  { get; set; }
    public int      QuestionsSubmitted { get; set; }
}
