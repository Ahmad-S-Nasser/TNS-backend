using MediatR;
using TipsAndSteps.Content.Domain.Enums;

namespace TipsAndSteps.Content.Application.Queries.GetArticle;

public sealed record GetArticleQuery(string ArticleId, string Language = "ar") : IRequest<ArticleDto?>;

public sealed record ArticleDto(
    string        Id,
    string        Section,
    string        Type,
    string        Status,
    string        Title,
    string        Body,
    string?       Summary,
    string?       ThumbnailUrl,
    string?       VideoUrl,
    List<string>  Tags,
    int           MinAgeMonths,
    int           MaxAgeMonths,
    int           ViewCount,
    double        AverageRating,
    DateTime      PublishedAt);
