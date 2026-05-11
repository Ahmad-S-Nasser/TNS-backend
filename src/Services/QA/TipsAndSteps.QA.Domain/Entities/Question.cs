namespace TipsAndSteps.QA.Domain.Entities;

public sealed class Question
{
    public string     Id             { get; set; } = Guid.NewGuid().ToString();
    public string     ParentId       { get; set; } = string.Empty;
    public string?    AssignedDoctorId { get; set; }
    public string     Category       { get; set; } = string.Empty;
    public string     QuestionTextAr { get; set; } = string.Empty;
    public string?    QuestionTextEn { get; set; }
    public bool       IsAnonymous    { get; set; }
    public string     Status         { get; set; } = "Pending"; // Pending | Answered | Closed
    public Answer?    Answer         { get; set; }
    public DateTime   SubmittedAt    { get; set; } = DateTime.UtcNow;
    public DateTime?  AnsweredAt     { get; set; }
}

public sealed class Answer
{
    public string   DoctorId   { get; set; } = string.Empty;
    public string   AnswerText { get; set; } = string.Empty;
    public DateTime AnsweredAt { get; set; } = DateTime.UtcNow;
}
