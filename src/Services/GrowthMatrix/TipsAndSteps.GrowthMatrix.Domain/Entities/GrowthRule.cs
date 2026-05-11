namespace TipsAndSteps.GrowthMatrix.Domain.Entities;

public class GrowthRule
{
    public string   Id                { get; set; } = string.Empty;
    public string   SkillId           { get; set; } = string.Empty;
    public string   AgeGroupId        { get; set; } = string.Empty;
    public int      ExpectedMonth     { get; set; }
    
    // For boolean metrics
    public bool?    ExpectedBoolean   { get; set; }

    // For numeric metrics
    public double?  MinValue          { get; set; }
    public double?  OptimalMin        { get; set; }
    public double?  OptimalMax        { get; set; }
    public double?  MaxValue          { get; set; }

    // For scale metrics
    public int?     MinScaleValue     { get; set; }
    public int?     OptimalScaleValue { get; set; }

    public DateTime CreatedAt         { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt         { get; set; } = DateTime.UtcNow;
}
