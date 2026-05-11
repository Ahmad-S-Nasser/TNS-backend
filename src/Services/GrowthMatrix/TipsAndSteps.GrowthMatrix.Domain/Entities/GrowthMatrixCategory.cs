namespace TipsAndSteps.GrowthMatrix.Domain.Entities;

public class GrowthMatrixCategory
{
    public string Id { get; set; } = string.Empty;
    public LocalizedString Name { get; set; } = new();
    public LocalizedString Description { get; set; } = new();
    public string IconKey { get; set; } = "ruler";
    public string? IconUrl { get; set; }
    public string? ImageUrl { get; set; }
    public string Color { get; set; } = "#3B82F6";
    public int SortOrder { get; set; }
}
