using MediatR;
using TipsAndSteps.Content.Application.Abstractions;
using TipsAndSteps.Content.Domain.Enums;

namespace TipsAndSteps.Content.Application.Queries.GetContentStats;

public sealed class GetContentStatsHandler : IRequestHandler<GetContentStatsQuery, ContentStatsDto>
{
    private readonly IContentReadRepository _readRepo;
    public GetContentStatsHandler(IContentReadRepository readRepo) => _readRepo = readRepo;

    public async Task<ContentStatsDto> Handle(GetContentStatsQuery request, CancellationToken ct)
    {
        var articles = await _readRepo.ListAllAsync(ct);
        
        var sectionStats = articles
            .GroupBy(a => a.Section)
            .Select(g => new SectionStatsDto(
                Section: g.Key,
                Total: g.Count(),
                Published: g.Count(a => a.Status == ContentStatus.Published),
                Draft: g.Count(a => a.Status == ContentStatus.Draft),
                Review: g.Count(a => a.Status == ContentStatus.Review),
                Approved: g.Count(a => a.Status == ContentStatus.Approved),
                Archived: g.Count(a => a.Status == ContentStatus.Archived)
            ))
            .ToList();

        return new ContentStatsDto(
            TotalCount: articles.Count,
            DraftCount: articles.Count(a => a.Status == ContentStatus.Draft),
            ReviewCount: articles.Count(a => a.Status == ContentStatus.Review),
            PublishedCount: articles.Count(a => a.Status == ContentStatus.Published),
            SectionStats: sectionStats
        );
    }
}
