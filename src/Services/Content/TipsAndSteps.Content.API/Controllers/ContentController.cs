using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TipsAndSteps.Content.Application.Abstractions;
using TipsAndSteps.Content.Application.Commands.CreateArticle;
using TipsAndSteps.Content.Application.Commands.UpdateArticle;
using TipsAndSteps.Content.Application.Commands.DeleteArticle;
using TipsAndSteps.Content.Application.Queries.GetArticle;
using TipsAndSteps.Content.Application.Queries.GetContentStats;
using TipsAndSteps.Content.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace TipsAndSteps.Content.API.Controllers;

[ApiController]
[Route("api/content")]
public sealed class ContentController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IContentReadRepository _readRepo;
    private readonly ILogger<ContentController> _logger;

    public ContentController(IMediator mediator, IContentReadRepository readRepo, ILogger<ContentController> logger)
    {
        _mediator = mediator;
        _readRepo = readRepo;
        _logger = logger;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> List(
        [FromQuery] string? section,
        [FromQuery] ContentStatus? status,
        [FromQuery] string? query,
        [FromQuery] string lang = "ar",
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        _logger.LogInformation("Listing content for section: {Section}, query: {Query}", section, query);
        if (!string.IsNullOrEmpty(query))
        {
            var searchResults = await _readRepo.SearchAsync(query, lang, page, pageSize, ct);
            return Ok(searchResults);
        }

        if (!string.IsNullOrEmpty(section))
        {
            // Try to parse the section string to the enum
            if (Enum.TryParse<ContentSection>(section, true, out var sectionEnum))
            {
                var sectionItems = await _readRepo.ListBySectionAsync(sectionEnum, page, pageSize, status, ct);
                return Ok(sectionItems);
            }
            // Handle kebab-case manually if needed, but TryParse with ignoreCase=true handles most simple ones
            // For sexual-education -> SexualEducation, we might need a map
            var mappedSection = section.ToLower() switch {
                "sexual-education" => ContentSection.SexualEducation,
                "educational-games" => ContentSection.EducationalGames,
                "health-units" => ContentSection.HealthUnits,
                "behavioral" => ContentSection.Behavioral,
                "psychological" => ContentSection.Psychological,
                "nutrition" => ContentSection.Nutrition,
                "hospitals" => ContentSection.Hospitals,
                "emergency" => ContentSection.Emergency,
                "vaccines" => ContentSection.Vaccines,
                "questionnaires" => ContentSection.Questionnaires,
                "faqs" => ContentSection.Faqs,
                _ => (ContentSection?)null
            };

            if (mappedSection.HasValue)
            {
                var sectionItems = await _readRepo.ListBySectionAsync(mappedSection.Value, page, pageSize, status, ct);
                return Ok(sectionItems);
            }
        }

        var all = await _readRepo.ListAllAsync(ct);
        if (status.HasValue)
        {
            all = all.Where(a => a.Status == status.Value).ToList();
        }
        return Ok(all.Skip((page - 1) * pageSize).Take(pageSize));
    }


    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetById(
        string id,
        [FromQuery] string lang = "ar",
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetArticleQuery(id, lang), ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create(
        [FromBody] CreateArticleCommand command,
        CancellationToken ct)
    {
        try 
        {
            var id = await _mediator.Send(command, ct);
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating content article");
            return StatusCode(500, new { error = "Failed to create content", detail = ex.Message });
        }
    }

    [HttpPatch("{id}")]
    [Authorize]
    public async Task<IActionResult> Update(
        string id,
        [FromBody] UpdateArticleCommand command,
        CancellationToken ct)
    {
        if (id != command.Id) return BadRequest("ID mismatch");

        try 
        {
            var updated = await _mediator.Send(command, ct);
            return updated ? NoContent() : NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating content article {Id}", id);
            return StatusCode(500, new { error = "Failed to update content", detail = ex.Message });
        }
    }

    [HttpPatch("{id}/status")]
    [Authorize]
    public async Task<IActionResult> UpdateStatus(
        string id,
        [FromBody] UpdateArticleStatusCommand command,
        CancellationToken ct)
    {
        if (command == null)
        {
            _logger.LogWarning("UpdateStatus received null command");
            return BadRequest("Command is null");
        }

        if (!ModelState.IsValid)
        {
            var errors = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            _logger.LogWarning("Validation failed for UpdateStatus: {Errors}", errors);
            return BadRequest(ModelState);
        }

        if (id != command.Id) 
        {
            _logger.LogWarning("ID mismatch for UpdateStatus: URL {UrlId} != Body {BodyId}", id, command.Id);
            return BadRequest("ID mismatch");
        }

        try 
        {
            var updated = await _mediator.Send(command, ct);
            return updated ? NoContent() : NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating status for content article {Id}", id);
            return StatusCode(500, new { error = "Failed to update content status", detail = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(string id, CancellationToken ct)
    {
        try 
        {
            var deleted = await _mediator.Send(new DeleteArticleCommand(id), ct);
            return deleted ? NoContent() : NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting content article {Id}", id);
            return StatusCode(500, new { error = "Failed to delete content", detail = ex.Message });
        }
    }

    [HttpGet("stats")]
    [Authorize]
    public async Task<IActionResult> GetStats(CancellationToken ct)
    {
        try 
        {
            var stats = await _mediator.Send(new GetContentStatsQuery(), ct);
            return Ok(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting content stats");
            return StatusCode(500, new { error = "Failed to get content stats", detail = ex.Message });
        }
    }

    [HttpGet("section/{section}")]
    [Authorize]
    public async Task<IActionResult> GetBySection(
        ContentSection section,
        [FromQuery] ContentStatus? status,
        [FromQuery] string lang = "ar",
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var items = await _readRepo.ListBySectionAsync(section, page, pageSize, status, ct);
        return Ok(items);
    }
}
