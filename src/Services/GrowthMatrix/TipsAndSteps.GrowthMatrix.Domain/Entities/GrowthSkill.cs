using TipsAndSteps.GrowthMatrix.Domain.Enums;

namespace TipsAndSteps.GrowthMatrix.Domain.Entities;

public sealed class GrowthSkill
{
    public string           Id              { get; set; } = string.Empty;
    public string           CategoryId      { get; set; } = string.Empty;
    public LocalizedString  Title           { get; set; } = new();
    public LocalizedString  Description     { get; set; } = new();
    public MetricType       MetricType      { get; set; }
    public string?          Unit            { get; set; }
    public int              Weight          { get; set; } = 1;
    public List<LocalizedString> ImprovementTips { get; set; } = new();
    public int              SortOrder       { get; set; }
    public DateTime         CreatedAt       { get; set; } = DateTime.UtcNow;
    public DateTime         UpdatedAt       { get; set; } = DateTime.UtcNow;
}
