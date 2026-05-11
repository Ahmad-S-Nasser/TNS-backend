using MediatR;
using TipsAndSteps.GrowthMatrix.Domain.Entities;

namespace TipsAndSteps.GrowthMatrix.Application.Commands.RecordAssessment;

public sealed record RecordAssessmentCommand(
    string ChildId,
    string ParentId,
    string AgeGroupId,
    List<SkillResponse> Responses
) : IRequest<RecordAssessmentResult>;

public sealed record RecordAssessmentResult(
    string AssessmentId,
    decimal TotalScore,
    string ScoreLevel,
    Dictionary<string, decimal> CategoryScores,
    List<string> Recommendations);
