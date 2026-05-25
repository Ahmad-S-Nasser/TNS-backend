namespace TipsAndSteps.QA.Domain.Enums;

/// <summary>
/// Known categories for Q&amp;A questions and FAQs.
/// These IDs are stable — use them as <c>categoryId</c> query parameters.
/// </summary>
public enum QACategory
{
    General             = 1,
    Nutrition           = 2,
    Growth              = 3,
    Behavioral          = 4,
    Health              = 5,
    Education           = 6,
    SexualEducation     = 7,
    Vaccines            = 8,
    Emergency           = 9,
}
