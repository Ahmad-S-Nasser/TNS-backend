using MediatR;

namespace TipsAndSteps.Content.Application.Queries.GetContentStats;

public sealed record GetContentStatsQuery() : IRequest<ContentStatsDto>;
