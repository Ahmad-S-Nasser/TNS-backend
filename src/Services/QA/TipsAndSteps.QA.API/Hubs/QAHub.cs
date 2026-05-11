using Microsoft.AspNetCore.SignalR;
using TipsAndSteps.QA.Domain.Entities;

namespace TipsAndSteps.QA.API.Hubs;

public interface IQAHudClient
{
    Task QuestionSubmitted(Question question);
    Task QuestionAnswered(Question question);
}

public sealed class QAHub : Hub<IQAHudClient>
{
    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
    }
}
