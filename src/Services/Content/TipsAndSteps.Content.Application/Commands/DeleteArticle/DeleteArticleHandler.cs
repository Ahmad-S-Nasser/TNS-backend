using MediatR;
using TipsAndSteps.Content.Application.Abstractions;

namespace TipsAndSteps.Content.Application.Commands.DeleteArticle;

public sealed class DeleteArticleHandler : IRequestHandler<DeleteArticleCommand, bool>
{
    private readonly IContentRepository _repo;
    public DeleteArticleHandler(IContentRepository repo) => _repo = repo;

    public async Task<bool> Handle(DeleteArticleCommand request, CancellationToken ct)
    {
        var existing = await _repo.FindByIdAsync(request.Id, ct);
        if (existing == null) return false;

        await _repo.DeleteAsync(request.Id, ct);
        return true;
    }
}
