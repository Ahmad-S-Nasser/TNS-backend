using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TipsAndSteps.GrowthMatrix.Application.Abstractions;
using TipsAndSteps.GrowthMatrix.Domain.Entities;
using TipsAndSteps.GrowthMatrix.Domain.Enums;

namespace TipsAndSteps.GrowthMatrix.API.Controllers;

[ApiController]
[Route("api/growth")]
[Authorize]
public class GrowthController : ControllerBase
{
    private readonly IGrowthAgeGroupRepository _ageGroups;
    private readonly IGrowthMatrixCategoryRepository _categories;
    private readonly IGrowthRuleRepository _rules;
    private readonly IGrowthSkillRepository _skills;
    private readonly IGrowthAssessmentRepository _assessments;

    public GrowthController(
        IGrowthAgeGroupRepository ageGroups,
        IGrowthMatrixCategoryRepository categories,
        IGrowthRuleRepository rules,
        IGrowthSkillRepository skills,
        IGrowthAssessmentRepository assessments)
    {
        _ageGroups = ageGroups;
        _categories = categories;
        _rules = rules;
        _skills = skills;
        _assessments = assessments;
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        var assessments = await _assessments.ListAsync();
        var skills = await _skills.ListAsync();
        var categories = await _categories.ListAsync();
        var ageGroups = await _ageGroups.ListAsync();
        var rules = await _rules.ListAsync();

        var skillsPerCategory = categories.Select(c => new {
            categoryId = c.Id,
            categoryName = c.Name,
            count = skills.Count(s => s.CategoryId == c.Id),
            color = c.Color
        }).ToList();

        var coverageByAgeGroup = ageGroups.Select(ag => {
            var agSkillsCount = skills.Count(); // For now, assume all skills apply to all age groups if rules exist
            var agRulesCount = rules.Count(r => r.AgeGroupId == ag.Id);
            return new {
                id = ag.Id,
                label = ag.Label,
                coverage = agSkillsCount > 0 ? (int)((decimal)agRulesCount / agSkillsCount * 100) : 0
            };
        }).ToList();

        var overallCoverage = skills.Count > 0 
            ? (int)((decimal)rules.Select(r => r.SkillId).Distinct().Count() / skills.Count * 100)
            : 0;

        return Ok(new
        {
            activeAgeGroups = ageGroups.Count,
            totalSkills = skills.Count,
            totalCategories = categories.Count,
            coverage = overallCoverage,
            totalAssessments = assessments.Count,
            normalGrowth = assessments.Count(a => a.ScoreLevel >= ScoreLevel.Good),
            atRisk = assessments.Count(a => a.ScoreLevel == ScoreLevel.NeedsAttention),
            critical = assessments.Count(a => a.ScoreLevel == ScoreLevel.RequiresIntervention),
            skillsPerCategory,
            coverageByAgeGroup
        });
    }

    [HttpGet("rules")]
    public async Task<IActionResult> GetRules([FromQuery] string? ageGroupId, [FromQuery] string? skillId) 
    {
        var rules = await _rules.ListAsync();
        if (!string.IsNullOrEmpty(ageGroupId)) rules = rules.Where(r => r.AgeGroupId == ageGroupId).ToList();
        if (!string.IsNullOrEmpty(skillId)) rules = rules.Where(r => r.SkillId == skillId).ToList();
        return Ok(rules);
    }

    /// <summary>
    /// Mobile-friendly catalog: all growth field categories enriched with skill and milestone counts.
    /// Use the returned <c>id</c> when calling GET /api/growth/fields/{id}.
    /// </summary>
    [HttpGet("fields")]
    public async Task<IActionResult> GetFields()
    {
        var categories = await _categories.ListAsync();
        var skills     = await _skills.ListAsync();
        var rules      = await _rules.ListAsync();

        var result = categories
            .OrderBy(c => c.SortOrder)
            .Select(c =>
            {
                var catSkills     = skills.Where(s => s.CategoryId == c.Id).ToList();
                var catSkillIds   = catSkills.Select(s => s.Id).ToHashSet();
                var milestoneCount = rules.Count(r => catSkillIds.Contains(r.SkillId));

                return new GrowthFieldSummaryDto(
                    Id:             c.Id,
                    NameAr:         c.Name.Ar,
                    NameEn:         c.Name.En,
                    DescriptionAr:  c.Description.Ar,
                    DescriptionEn:  c.Description.En,
                    IconKey:        c.IconKey,
                    IconUrl:        c.IconUrl,
                    ImageUrl:       c.ImageUrl,
                    Color:          c.Color,
                    SortOrder:      c.SortOrder,
                    SkillCount:     catSkills.Count,
                    MilestoneCount: milestoneCount);
            });

        return Ok(result);
    }

    /// <summary>
    /// Mobile-friendly field detail: single category with its full skill list, per-skill milestones,
    /// and improvement tips aggregated for easy Flutter consumption.
    /// </summary>
    [HttpGet("fields/{id}")]
    public async Task<IActionResult> GetFieldById(string id)
    {
        var categories = await _categories.ListAsync();
        var category   = categories.FirstOrDefault(c => c.Id == id);
        if (category is null) return NotFound();

        var skills = (await _skills.ListAsync())
            .Where(s => s.CategoryId == id)
            .OrderBy(s => s.SortOrder)
            .ToList();

        var allRules   = await _rules.ListAsync();
        var skillIds   = skills.Select(s => s.Id).ToHashSet();
        var categoryRules = allRules.Where(r => skillIds.Contains(r.SkillId)).ToList();

        var skillDtos = skills.Select(skill =>
        {
            var skillRules = categoryRules
                .Where(r => r.SkillId == skill.Id)
                .Select(r => new GrowthMilestoneDto(
                    RuleId:            r.Id,
                    ExpectedMonth:     r.ExpectedMonth,
                    ExpectedBoolean:   r.ExpectedBoolean,
                    MinValue:          r.MinValue,
                    OptimalMin:        r.OptimalMin,
                    OptimalMax:        r.OptimalMax,
                    MaxValue:          r.MaxValue,
                    MinScaleValue:     r.MinScaleValue,
                    OptimalScaleValue: r.OptimalScaleValue))
                .OrderBy(r => r.ExpectedMonth)
                .ToList();

            var tips = skill.ImprovementTips
                .Select(t => new LocalizedStringDto(t.En, t.Ar))
                .ToList();

            return new GrowthFieldSkillDto(
                Id:          skill.Id,
                TitleAr:     skill.Title.Ar,
                TitleEn:     skill.Title.En,
                DescriptionAr: skill.Description.Ar,
                DescriptionEn: skill.Description.En,
                MetricType:  skill.MetricType.ToString(),
                Unit:        skill.Unit,
                Weight:      skill.Weight,
                SortOrder:   skill.SortOrder,
                Milestones:  skillRules,
                ImprovementTips: tips);
        }).ToList();

        var dto = new GrowthFieldDetailDto(
            Id:             category.Id,
            NameAr:         category.Name.Ar,
            NameEn:         category.Name.En,
            DescriptionAr:  category.Description.Ar,
            DescriptionEn:  category.Description.En,
            IconKey:        category.IconKey,
            IconUrl:        category.IconUrl,
            ImageUrl:       category.ImageUrl,
            Color:          category.Color,
            SortOrder:      category.SortOrder,
            Skills:         skillDtos);

        return Ok(dto);
    }

    [HttpGet("age-groups")]
    public async Task<IActionResult> GetAgeGroups() => Ok(await _ageGroups.ListAsync());

    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories() => Ok(await _categories.ListAsync());

    [HttpGet("skills")]
    public async Task<IActionResult> GetSkills([FromQuery] string? categoryId) 
    {
        var skills = await _skills.ListAsync();
        if (!string.IsNullOrEmpty(categoryId))
        {
            skills = skills.Where(s => s.CategoryId == categoryId).ToList();
        }
        return Ok(skills);
    }

    [HttpPost("age-groups")]
    public async Task<IActionResult> CreateAgeGroup([FromBody] GrowthAgeGroup ageGroup)
    {
        await _ageGroups.CreateAsync(ageGroup);
        return Created("", ageGroup);
    }

    [HttpPost("categories")]
    public async Task<IActionResult> CreateCategory([FromBody] GrowthMatrixCategory category)
    {
        await _categories.CreateAsync(category);
        return Created("", category);
    }

    [HttpPost("skills")]
    public async Task<IActionResult> CreateSkill([FromBody] GrowthSkill skill)
    {
        await _skills.CreateAsync(skill);
        return Created("", skill);
    }

    [HttpPatch("skills/{id}")]
    public async Task<IActionResult> UpdateSkill(string id, [FromBody] GrowthSkill skill)
    {
        skill.Id = id;
        await _skills.UpdateAsync(id, skill);
        return Ok(skill);
    }

    [HttpDelete("skills/{id}")]
    public async Task<IActionResult> DeleteSkill(string id)
    {
        await _skills.DeleteAsync(id);
        return NoContent();
    }

    [HttpPost("rules")]
    public async Task<IActionResult> CreateRule([FromBody] GrowthRule rule)
    {
        await _rules.CreateAsync(rule);
        return Created("", rule);
    }

    [HttpPatch("rules/{id}")]
    public async Task<IActionResult> UpdateRule(string id, [FromBody] GrowthRule rule)
    {
        rule.Id = id;
        await _rules.UpdateAsync(id, rule);
        return Ok(rule);
    }

    [HttpDelete("rules/{id}")]
    public async Task<IActionResult> DeleteRule(string id)
    {
        await _rules.DeleteAsync(id);
        return NoContent();
    }

    [HttpPatch("age-groups/{id}")]
    public async Task<IActionResult> UpdateAgeGroup(string id, [FromBody] GrowthAgeGroup ageGroup)
    {
        ageGroup.Id = id;
        await _ageGroups.UpdateAsync(id, ageGroup);
        return Ok(ageGroup);
    }

    [HttpDelete("age-groups/{id}")]
    public async Task<IActionResult> DeleteAgeGroup(string id)
    {
        await _ageGroups.DeleteAsync(id);
        return NoContent();
    }

    [HttpPatch("categories/{id}")]
    public async Task<IActionResult> UpdateCategory(string id, [FromBody] GrowthMatrixCategory category)
    {
        category.Id = id;
        await _categories.UpdateAsync(id, category);
        return Ok(category);
    }

    [HttpDelete("categories/{id}")]
    public async Task<IActionResult> DeleteCategory(string id)
    {
        await _categories.DeleteAsync(id);
        return NoContent();
    }
}

// ──────────────────────────────────────────────────────────────────────────
// DTOs for GET /api/growth/fields and GET /api/growth/fields/{id}
// ──────────────────────────────────────────────────────────────────────────

public sealed record LocalizedStringDto(string En, string Ar);

public sealed record GrowthFieldSummaryDto(
    string  Id,
    string  NameAr,
    string  NameEn,
    string  DescriptionAr,
    string  DescriptionEn,
    string  IconKey,
    string? IconUrl,
    string? ImageUrl,
    string  Color,
    int     SortOrder,
    int     SkillCount,
    int     MilestoneCount);

public sealed record GrowthMilestoneDto(
    string  RuleId,
    int     ExpectedMonth,
    bool?   ExpectedBoolean,
    double? MinValue,
    double? OptimalMin,
    double? OptimalMax,
    double? MaxValue,
    int?    MinScaleValue,
    int?    OptimalScaleValue);

public sealed record GrowthFieldSkillDto(
    string  Id,
    string  TitleAr,
    string  TitleEn,
    string  DescriptionAr,
    string  DescriptionEn,
    string  MetricType,
    string? Unit,
    int     Weight,
    int     SortOrder,
    List<GrowthMilestoneDto>    Milestones,
    List<LocalizedStringDto>    ImprovementTips);

public sealed record GrowthFieldDetailDto(
    string  Id,
    string  NameAr,
    string  NameEn,
    string  DescriptionAr,
    string  DescriptionEn,
    string  IconKey,
    string? IconUrl,
    string? ImageUrl,
    string  Color,
    int     SortOrder,
    List<GrowthFieldSkillDto> Skills);
