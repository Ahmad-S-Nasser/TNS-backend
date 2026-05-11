using MediatR;
using TipsAndSteps.Content.Domain.Enums;

namespace TipsAndSteps.Content.Application.Commands.UpdateArticle;

public sealed class UpdateArticleStatusCommand : IRequest<bool>
{
    public string Id { get; set; } = string.Empty;
    public ContentStatus Status { get; set; }
}
