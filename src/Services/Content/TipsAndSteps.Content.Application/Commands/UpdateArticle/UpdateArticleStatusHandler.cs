using MediatR;
using TipsAndSteps.Content.Application.Abstractions;
using TipsAndSteps.Content.Domain.Enums;

namespace TipsAndSteps.Content.Application.Commands.UpdateArticle;

public sealed class UpdateArticleStatusHandler : IRequestHandler<UpdateArticleStatusCommand, bool>
{
    private readonly IContentRepository      _repo;
    private readonly IContentEventPublisher _events;

    public UpdateArticleStatusHandler(IContentRepository repo, IContentEventPublisher events)
        => (_repo, _events) = (repo, events);

    public async Task<bool> Handle(UpdateArticleStatusCommand request, CancellationToken ct)
    {
        var article = await _repo.FindByIdAsync(request.Id, ct);
        if (article == null) return false;

        article.Status = request.Status;
        if (request.Status == ContentStatus.Published && article.PublishedAt == null)
        {
            article.PublishedAt = DateTime.UtcNow;
            // Fire and forget to avoid blocking on unreachable Kafka
            _ = Task.Run(async () => 
            {
                try 
                {
                    // Use CancellationToken.None since the request's token will be cancelled
                    await _events.PublishContentPublishedAsync(article, CancellationToken.None);
                }
                catch (Exception ex)
                {
                    // Log failure in background if needed
                }
            });
        }

        await _repo.UpdateAsync(article, ct);
        return true;
    }
}
