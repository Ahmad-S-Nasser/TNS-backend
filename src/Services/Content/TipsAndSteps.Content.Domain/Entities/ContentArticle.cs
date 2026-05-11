using TipsAndSteps.Content.Domain.Enums;

namespace TipsAndSteps.Content.Domain.Entities;

/// <summary>
/// Bilingual content article for mothers and children (ar-EG + en-US).
/// </summary>
public sealed class ContentArticle
{
    public string         Id              { get; set; } = Guid.NewGuid().ToString();
    public ContentSection Section         { get; set; }
    public ContentType    Type            { get; set; }
    public ContentStatus  Status          { get; set; } = ContentStatus.Draft;

    // Bilingual fields
    public string         TitleAr         { get; set; } = string.Empty;
    public string         TitleEn         { get; set; } = string.Empty;
    public string         BodyAr          { get; set; } = string.Empty;
    public string         BodyEn          { get; set; } = string.Empty;
    public string?        SummaryAr       { get; set; }
    public string?        SummaryEn       { get; set; }

    // Media
    public string?        ThumbnailUrl    { get; set; }
    public string?        VideoUrl        { get; set; }
    public List<string>   ImageUrls       { get; set; } = [];
    public List<string>   Tags            { get; set; } = [];
    public Dictionary<string, object> Metadata { get; set; } = [];

    // Age targeting
    public int            MinAgeMonths    { get; set; } = 0;
    public int            MaxAgeMonths    { get; set; } = 36; // 0-3 years

    // Metadata
    public string         AuthorId        { get; set; } = string.Empty;
    public string?        ReviewedBy      { get; set; }
    public int            ViewCount       { get; set; }
    public double         AverageRating   { get; set; }
    public DateTime       CreatedAt       { get; set; } = DateTime.UtcNow;
    public DateTime       UpdatedAt       { get; set; } = DateTime.UtcNow;
    public DateTime?      PublishedAt     { get; set; }
}
