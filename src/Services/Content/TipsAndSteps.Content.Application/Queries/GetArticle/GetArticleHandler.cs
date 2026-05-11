using MediatR;
using TipsAndSteps.Content.Application.Abstractions;

namespace TipsAndSteps.Content.Application.Queries.GetArticle;

public sealed class GetArticleHandler : IRequestHandler<GetArticleQuery, ArticleDto?>
{
    private readonly IContentReadRepository _repo;
    private readonly IContentCacheService   _cache;

    public GetArticleHandler(IContentReadRepository repo, IContentCacheService cache)
        => (_repo, _cache) = (repo, cache);

    public async Task<ArticleDto?> Handle(GetArticleQuery request, CancellationToken ct)
    {
        // Check Redis cache first
        var cacheKey = $"content:{request.ArticleId}:{request.Language}";
        var cached   = await _cache.GetAsync<ArticleDto>(cacheKey, ct);
        if (cached is not null) return cached;

        var article = await _repo.FindByIdAsync(request.ArticleId, ct);
        if (article is null) return null;

        bool isAr = request.Language == "ar";
        var dto = new ArticleDto(
            article.Id,
            article.Section.ToString(),
            article.Type.ToString(),
            article.Status.ToString(),
            isAr ? article.TitleAr : article.TitleEn,
            isAr ? article.BodyAr  : article.BodyEn,
            isAr ? article.SummaryAr : article.SummaryEn,
            article.ThumbnailUrl,
            article.VideoUrl,
            article.Tags,
            article.MinAgeMonths,
            article.MaxAgeMonths,
            article.ViewCount,
            article.AverageRating,
            article.PublishedAt ?? article.CreatedAt);

        // Cache for 30 min
        await _cache.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(30), ct);
        return dto;
    }
}
