using TipsAndSteps.Content.Domain.Enums;

namespace TipsAndSteps.Content.Application.Queries.GetContentStats;

public sealed record SectionStatsDto(
    ContentSection Section,
    int Total,
    int Published,
    int Draft,
    int Review,
    int Approved,
    int Archived
);

public sealed record ContentStatsDto(
    int TotalCount,
    int DraftCount,
    int ReviewCount,
    int PublishedCount,
    List<SectionStatsDto> SectionStats
);
