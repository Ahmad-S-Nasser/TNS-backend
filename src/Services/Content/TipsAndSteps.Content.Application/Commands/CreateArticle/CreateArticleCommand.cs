using MediatR;
using TipsAndSteps.Content.Domain.Enums;

namespace TipsAndSteps.Content.Application.Commands.CreateArticle;

public sealed record CreateArticleCommand(
    ContentSection Section,
    ContentType    Type,
    string         TitleAr,
    string         TitleEn,
    string         BodyAr,
    string         BodyEn,
    string?        SummaryAr,
    string?        SummaryEn,
    string?        ThumbnailUrl,
    string?        VideoUrl,
    List<string>   Tags,
    int            MinAgeMonths,
    int            MaxAgeMonths,
    ContentStatus  Status,
    string         AuthorId,
    Dictionary<string, object>? Metadata = null
) : IRequest<string>; // returns new article ID
