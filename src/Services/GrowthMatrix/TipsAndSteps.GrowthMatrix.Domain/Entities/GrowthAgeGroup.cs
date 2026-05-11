using TipsAndSteps.GrowthMatrix.Domain.Enums;

namespace TipsAndSteps.GrowthMatrix.Domain.Entities;

public class GrowthAgeGroup
{
    public string Id { get; set; } = string.Empty;
    public LocalizedString Label { get; set; } = new();
    public int MonthStart { get; set; }
    public int MonthEnd { get; set; }
    public LocalizedString Description { get; set; } = new();
    public AgeGroupStatus Status { get; set; } = AgeGroupStatus.Active;
}
