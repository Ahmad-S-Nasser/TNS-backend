using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TipsAndSteps.QA.Application.Commands.AnswerQuestion;
using TipsAndSteps.QA.Application.Queries.GetQuestions;

namespace TipsAndSteps.QA.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class QAController : ControllerBase
{
    private readonly IMediator _mediator;
    public QAController(IMediator mediator) => _mediator = mediator;

    [HttpGet("questions")]
    public async Task<IActionResult> GetQuestions(CancellationToken ct)
    {
        var questions = await _mediator.Send(new GetQuestionsQuery(), ct);
        return Ok(questions);
    }

    [HttpPost("questions/{id}/answer")]
    public async Task<IActionResult> AnswerQuestion(string id, [FromBody] AnswerRequest request, CancellationToken ct)
    {
        var command = new AnswerQuestionCommand(id, request.DoctorId, request.AnswerText);
        var result = await _mediator.Send(command, ct);
        return result ? NoContent() : NotFound();
    }

    [HttpGet("faqs")]
    public IActionResult GetFAQs()
    {
        return Ok(new[] {
            new { 
                id = "1", 
                question = new { en = "What is TipsAndSteps?", ar = "ما هو خطوات ونصائح؟" }, 
                answer = new { en = "A platform for child growth monitoring.", ar = "منصة لمراقبة نمو الطفل." },
                category = "General"
            }
        });
    }

    [HttpGet("questionnaires")]
    public IActionResult GetQuestionnaires()
    {
        return Ok(new[] {
            new { 
                id = "1", 
                title = new { en = "Health Assessment", ar = "تقييم الصحة" }, 
                description = new { en = "Standard health screening.", ar = "فحص صحي قياسي." },
                isPublished = true
            }
        });
    }
}

public record AnswerRequest(string DoctorId, string AnswerText);
