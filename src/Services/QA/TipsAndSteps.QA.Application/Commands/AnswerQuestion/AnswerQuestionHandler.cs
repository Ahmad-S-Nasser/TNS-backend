using MediatR;
using TipsAndSteps.QA.Application.Abstractions;
using TipsAndSteps.QA.Domain.Entities;

namespace TipsAndSteps.QA.Application.Commands.AnswerQuestion;

public sealed record AnswerQuestionCommand(string QuestionId, string DoctorId, string AnswerText) : IRequest<bool>;

public sealed class AnswerQuestionHandler : IRequestHandler<AnswerQuestionCommand, bool>
{
    private readonly IQARepository _repo;
    private readonly IQANotifier   _notifier;
    public AnswerQuestionHandler(IQARepository repo, IQANotifier notifier) 
        => (_repo, _notifier) = (repo, notifier);

    public async Task<bool> Handle(AnswerQuestionCommand request, CancellationToken ct)
    {
        var question = await _repo.GetByIdAsync(request.QuestionId, ct);
        if (question == null) return false;

        question.Answer = new Answer
        {
            DoctorId = request.DoctorId,
            AnswerText = request.AnswerText,
            AnsweredAt = DateTime.UtcNow
        };
        question.Status = "Answered";
        question.AnsweredAt = DateTime.UtcNow;

        await _repo.UpdateAsync(question, ct);
        await _notifier.NotifyQuestionAnsweredAsync(question);
        return true;
    }
}
