using MediatR;
using TipsAndSteps.Content.Application.Abstractions;
using System.Linq;
using TipsAndSteps.Content.Domain.Entities;
using TipsAndSteps.Content.Domain.Enums;

namespace TipsAndSteps.Content.Application.Commands.CreateArticle;

public sealed class CreateArticleHandler : IRequestHandler<CreateArticleCommand, string>
{
    private readonly IContentRepository     _repo;
    private readonly IContentEventPublisher _events;

    public CreateArticleHandler(IContentRepository repo, IContentEventPublisher events)
        => (_repo, _events) = (repo, events);

    public async Task<string> Handle(CreateArticleCommand request, CancellationToken ct)
    {
        var article = new ContentArticle
        {
            Section      = request.Section,
            Type         = request.Type,
            TitleAr      = request.TitleAr,
            TitleEn      = request.TitleEn,
            BodyAr       = request.BodyAr,
            BodyEn       = request.BodyEn,
            SummaryAr    = request.SummaryAr,
            SummaryEn    = request.SummaryEn,
            ThumbnailUrl = request.ThumbnailUrl,
            VideoUrl     = request.VideoUrl,
            Tags         = request.Tags,
            MinAgeMonths = request.MinAgeMonths,
            MaxAgeMonths = request.MaxAgeMonths,
            Status       = request.Status,
            AuthorId     = request.AuthorId,
            Metadata     = ProcessMetadata(request.Metadata)
        };

        // Automatic publishing for non-medical sections
        var nonMedicalSections = new[] { ContentSection.EducationalGames, ContentSection.Questionnaires, ContentSection.Faqs };
        if (nonMedicalSections.Contains(article.Section))
        {
            article.Status = ContentStatus.Published;
            article.PublishedAt = DateTime.UtcNow;
        }

        await _repo.CreateAsync(article, ct);
        return article.Id;
    }

    private Dictionary<string, object> ProcessMetadata(Dictionary<string, object>? metadata)
    {
        if (metadata == null) return new();
        var processed = new Dictionary<string, object>();
        foreach (var kvp in metadata)
        {
            processed[kvp.Key] = kvp.Value is System.Text.Json.JsonElement element 
                ? ConvertJsonElement(element) 
                : kvp.Value;
        }
        return processed;
    }

    private object ConvertJsonElement(System.Text.Json.JsonElement element)
    {
        return element.ValueKind switch
        {
            System.Text.Json.JsonValueKind.String => element.GetString()!,
            System.Text.Json.JsonValueKind.Number => element.TryGetInt32(out int i) ? i : (element.TryGetInt64(out long l) ? l : element.GetDouble()),
            System.Text.Json.JsonValueKind.True => true,
            System.Text.Json.JsonValueKind.False => false,
            System.Text.Json.JsonValueKind.Null => null!,
            System.Text.Json.JsonValueKind.Object => element.EnumerateObject().ToDictionary(p => p.Name, p => ConvertJsonElement(p.Value)),
            System.Text.Json.JsonValueKind.Array => element.EnumerateArray().Select(ConvertJsonElement).ToList(),
            _ => element.ToString()
        };
    }
}
