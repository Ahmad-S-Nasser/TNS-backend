using MediatR;
using TipsAndSteps.Content.Domain.Enums;

namespace TipsAndSteps.Content.Application.Commands.UpdateArticle;

public sealed record UpdateArticleCommand(
    string         Id,
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
    Dictionary<string, object>? Metadata = null
) : IRequest<bool>; // returns true if found and updated
