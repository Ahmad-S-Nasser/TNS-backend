using MediatR;
using TipsAndSteps.GrowthMatrix.Application.Abstractions;
using TipsAndSteps.GrowthMatrix.Application.Services;
using TipsAndSteps.GrowthMatrix.Domain.Entities;

namespace TipsAndSteps.GrowthMatrix.Application.Commands.RecordAssessment;

public sealed class RecordAssessmentHandler
    : IRequestHandler<RecordAssessmentCommand, RecordAssessmentResult>
{
    private readonly IGrowthSkillRepository    _skills;
    private readonly IGrowthAssessmentRepository _assessments;
    private readonly IGrowthEventPublisher     _events;
    private readonly GrowthScoringEngine       _engine;

    public RecordAssessmentHandler(
        IGrowthSkillRepository skills,
        IGrowthAssessmentRepository assessments,
        IGrowthEventPublisher events,
        GrowthScoringEngine engine)
    {
        _skills      = skills;
        _assessments = assessments;
        _events      = events;
        _engine      = engine;
    }

    public async Task<RecordAssessmentResult> Handle(
        RecordAssessmentCommand request, CancellationToken ct)
    {
        // Load all skills
        var skills = await _skills.ListAsync(ct);

        // Build assessment
        var assessment = new GrowthAssessment
        {
            ChildId   = request.ChildId,
            ParentId  = request.ParentId,
            AgeGroupId = request.AgeGroupId,
            Responses = request.Responses
        };

        // Calculate scores
        var result = _engine.Calculate(assessment, skills);
        assessment.TotalScore     = result.TotalScore;
        assessment.ScoreLevel     = result.Level;
        assessment.CategoryScores = result.CategoryScores;

        // Persist (write primary)
        await _assessments.CreateAsync(assessment, ct);

        // Publish to Kafka → Analytics + Notification services
        await _events.PublishAssessmentCompletedAsync(assessment, ct);

        return new RecordAssessmentResult(
            assessment.Id,
            result.TotalScore,
            result.Level.ToString(),
            result.CategoryScores,
            result.RecommendedActions);
    }
}
