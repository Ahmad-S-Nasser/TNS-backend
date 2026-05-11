using TipsAndSteps.QA.Domain.Entities;

namespace TipsAndSteps.QA.Application.Abstractions;

public interface IQANotifier
{
    Task NotifyQuestionSubmittedAsync(Question question);
    Task NotifyQuestionAnsweredAsync(Question question);
}
