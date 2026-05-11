using Microsoft.AspNetCore.SignalR;
using TipsAndSteps.QA.API.Hubs;
using TipsAndSteps.QA.Application.Abstractions;
using TipsAndSteps.QA.Domain.Entities;

namespace TipsAndSteps.QA.API.Services;

public sealed class QANotifier : IQANotifier
{
    private readonly IHubContext<QAHub, IQAHudClient> _hubContext;
    public QANotifier(IHubContext<QAHub, IQAHudClient> hubContext) => _hubContext = hubContext;

    public Task NotifyQuestionSubmittedAsync(Question question)
        => _hubContext.Clients.All.QuestionSubmitted(question);

    public Task NotifyQuestionAnsweredAsync(Question question)
        => _hubContext.Clients.All.QuestionAnswered(question);
}
