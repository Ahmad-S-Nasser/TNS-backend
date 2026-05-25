using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TipsAndSteps.GrowthMatrix.Application.Commands.RecordAssessment;

namespace TipsAndSteps.GrowthMatrix.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class AssessmentsController : ControllerBase
{
    private readonly IMediator _mediator;
    public AssessmentsController(IMediator mediator) => _mediator = mediator;

    /// <summary>Submit a growth matrix assessment for a child</summary>
    [HttpPost]
    [Authorize(Roles = "parent")]
    [ProducesResponseType(typeof(RecordAssessmentResult), StatusCodes.Status201Created)]
    public async Task<IActionResult> RecordAssessment(
        [FromBody] RecordAssessmentCommand command,
        CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return Created($"/api/assessments/{result.AssessmentId}", result);
    }

    /// <summary>Get assessment report for a child</summary>
    [HttpGet("child/{childId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetByChild(string childId)
        => Ok(new { message = $"Assessment history for child {childId} — implement via GetAssessmentReportQuery" });
}
