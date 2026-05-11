using MediatR;

namespace TipsAndSteps.Content.Application.Commands.DeleteArticle;

public sealed record DeleteArticleCommand(string Id) : IRequest<bool>;
