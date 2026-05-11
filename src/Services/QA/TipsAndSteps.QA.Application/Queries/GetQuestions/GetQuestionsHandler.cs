using MediatR;
using TipsAndSteps.QA.Application.Abstractions;
using TipsAndSteps.QA.Domain.Entities;

namespace TipsAndSteps.QA.Application.Queries.GetQuestions;

public sealed record GetQuestionsQuery() : IRequest<IReadOnlyList<Question>>;

public sealed class GetQuestionsHandler : IRequestHandler<GetQuestionsQuery, IReadOnlyList<Question>>
{
    private readonly IQARepository _repo;
    public GetQuestionsHandler(IQARepository repo) => _repo = repo;

    public Task<IReadOnlyList<Question>> Handle(GetQuestionsQuery request, CancellationToken ct)
        => _repo.GetAllAsync(ct);
}
