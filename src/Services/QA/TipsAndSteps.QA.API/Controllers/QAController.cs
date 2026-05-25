using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TipsAndSteps.QA.Application.Commands.AnswerQuestion;
using TipsAndSteps.QA.Application.Queries.GetQuestions;
using TipsAndSteps.QA.Domain.Enums;

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

    /// <summary>
    /// Returns the catalog of all Q&amp;A categories with their numeric IDs.
    /// Use the returned <c>id</c> as <c>categoryId</c> when calling GET /api/qa/faqs.
    /// </summary>
    [HttpGet("categories")]
    [ProducesResponseType(typeof(IEnumerable<QACategoryDto>), StatusCodes.Status200OK)]
    public IActionResult GetCategories()
    {
        var categories = Enum.GetValues<QACategory>()
            .Select(c => new QACategoryDto((int)c, c.ToString(), ToCategoryLabel(c)))
            .OrderBy(c => c.Id);
        return Ok(categories);
    }

    /// <summary>
    /// Returns FAQs, optionally filtered by <paramref name="categoryId"/>.
    /// Pass the numeric <c>id</c> from GET /api/qa/categories.
    /// </summary>
    [HttpGet("faqs")]
    [ProducesResponseType(typeof(IEnumerable<FaqDto>), StatusCodes.Status200OK)]
    public IActionResult GetFAQs([FromQuery] int? categoryId)
    {
        var allFaqs = GetAllFaqs();

        if (categoryId.HasValue)
        {
            if (!Enum.IsDefined(typeof(QACategory), categoryId.Value))
                return BadRequest(new { message = $"Unknown categoryId: {categoryId.Value}" });

            allFaqs = allFaqs.Where(f => f.CategoryId == categoryId.Value).ToList();
        }

        return Ok(allFaqs);
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

    // ── Helpers ────────────────────────────────────────────────────────────

    private static string ToCategoryLabel(QACategory category) => category switch
    {
        QACategory.General          => "عام / General",
        QACategory.Nutrition        => "تغذية / Nutrition",
        QACategory.Growth           => "نمو / Growth",
        QACategory.Behavioral       => "سلوك / Behavioral",
        QACategory.Health           => "صحة / Health",
        QACategory.Education        => "تعليم / Education",
        QACategory.SexualEducation  => "التربية الجنسية / Sexual Education",
        QACategory.Vaccines         => "لقاحات / Vaccines",
        QACategory.Emergency        => "طوارئ / Emergency",
        _                           => category.ToString()
    };

    private static List<FaqDto> GetAllFaqs() =>
    [
        new FaqDto("1", (int)QACategory.General,
            new LocalizedText("ما هو تطبيق خطوات ونصائح؟", "What is TipsAndSteps?"),
            new LocalizedText("منصة متكاملة لمتابعة نمو الطفل وتقديم النصائح الصحية.", "A comprehensive platform for child growth monitoring and health guidance.")),

        new FaqDto("2", (int)QACategory.Nutrition,
            new LocalizedText("ما هي الأطعمة المناسبة للرضيع في الأشهر الأولى؟", "What foods are suitable for an infant in the first months?"),
            new LocalizedText("الرضاعة الطبيعية هي الخيار الأمثل في الأشهر الستة الأولى.", "Breastfeeding is the optimal choice for the first six months.")),

        new FaqDto("3", (int)QACategory.Growth,
            new LocalizedText("ما هو الوزن الطبيعي للطفل في عمر 6 أشهر؟", "What is the normal weight for a baby at 6 months?"),
            new LocalizedText("في المتوسط بين 6.4 و 8 كيلوغرام، لكن يتفاوت بين الأطفال.", "On average between 6.4 and 8 kg, but it varies between children.")),

        new FaqDto("4", (int)QACategory.Behavioral,
            new LocalizedText("كيف أتعامل مع نوبات الغضب عند الطفل؟", "How do I handle temper tantrums in a child?"),
            new LocalizedText("حافظ على الهدوء وأعطِه مساحة ثم تحدث معه بعد هدوئه.", "Stay calm, give him space, then talk to him after he calms down.")),

        new FaqDto("5", (int)QACategory.Vaccines,
            new LocalizedText("ما هي اللقاحات الضرورية للطفل في السنة الأولى؟", "What vaccines are essential for a child in the first year?"),
            new LocalizedText("تشمل لقاحات BCG والهيباتيس B والروتا والسداسي وغيرها حسب الجدول الوطني.", "Includes BCG, Hepatitis B, Rotavirus, hexavalent, and others per the national schedule.")),

        new FaqDto("6", (int)QACategory.Health,
            new LocalizedText("ما هي أعراض التسنين عند الأطفال؟", "What are the symptoms of teething in children?"),
            new LocalizedText("إفراز اللعاب، الانزعاج، الرغبة في العض، وارتفاع طفيف في الحرارة.", "Drooling, irritability, desire to bite, and a slight fever.")),
    ];
}

// ── DTOs ──────────────────────────────────────────────────────────────────

public sealed record QACategoryDto(int Id, string Name, string Label);

public sealed record LocalizedText(string Ar, string En);

public sealed record FaqDto(string Id, int CategoryId, LocalizedText Question, LocalizedText Answer);

public record AnswerRequest(string DoctorId, string AnswerText);
