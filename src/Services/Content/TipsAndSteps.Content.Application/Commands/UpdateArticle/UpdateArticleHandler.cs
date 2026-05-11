using MediatR;
using TipsAndSteps.Content.Application.Abstractions;
using System.Linq;
using TipsAndSteps.Content.Domain.Entities;

namespace TipsAndSteps.Content.Application.Commands.UpdateArticle;

public sealed class UpdateArticleHandler : IRequestHandler<UpdateArticleCommand, bool>
{
    private readonly IContentRepository _repo;

    public UpdateArticleHandler(IContentRepository repo) => _repo = repo;

    public async Task<bool> Handle(UpdateArticleCommand request, CancellationToken ct)
    {
        var existing = await _repo.FindByIdAsync(request.Id, ct);
        if (existing == null) return false;

        existing.Section      = request.Section;
        existing.Type         = request.Type;
        existing.TitleAr      = request.TitleAr;
        existing.TitleEn      = request.TitleEn;
        existing.BodyAr       = request.BodyAr;
        existing.BodyEn       = request.BodyEn;
        existing.SummaryAr    = request.SummaryAr;
        existing.SummaryEn    = request.SummaryEn;
        existing.ThumbnailUrl = request.ThumbnailUrl;
        existing.VideoUrl     = request.VideoUrl;
        existing.Tags         = request.Tags;
        existing.MinAgeMonths = request.MinAgeMonths;
        existing.MaxAgeMonths = request.MaxAgeMonths;
        existing.Status       = request.Status;
        existing.Metadata     = ProcessMetadata(request.Metadata ?? existing.Metadata);
        existing.UpdatedAt    = DateTime.UtcNow;

        await _repo.UpdateAsync(existing, ct);
        return true;
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
