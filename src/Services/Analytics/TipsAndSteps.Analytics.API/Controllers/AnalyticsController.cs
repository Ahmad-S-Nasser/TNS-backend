using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TipsAndSteps.Analytics.Infrastructure.Persistence;

namespace TipsAndSteps.Analytics.API.Controllers;

[ApiController]
[Route("api/analytics")]
[Authorize]
public class AnalyticsController : ControllerBase
{
    private readonly IAnalyticsRepository _repo;

    public AnalyticsController(IAnalyticsRepository repo) => _repo = repo;

    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboardData(CancellationToken ct)
    {
        var kpis = await _repo.GetAllKPIsAsync(ct);
        var growth = await _repo.GetMonthlyGrowthAsync(6, ct);
        var engagement = await _repo.GetContentEngagementAsync(ct);

        var kpiMap = kpis.ToDictionary(k => k.Key, k => k.Value);
        
        // Ensure defaults for KPIs
        string[] required = { "totalUsers", "totalContent", "engagementRate" };
        foreach (var key in required)
        {
            if (!kpiMap.ContainsKey(key)) kpiMap[key] = 0;
        }

        return Ok(new
        {
            kpis = kpiMap,
            monthlyData = growth.OrderBy(m => m.YearMonth),
            engagementByContent = engagement
        });
    }

    [HttpGet("kpis")]
    public async Task<IActionResult> GetKPIs()
    {
        var kpis = await _repo.GetAllKPIsAsync();
        var result = kpis.ToDictionary(k => k.Key, k => k.Value);
        return Ok(result);
    }
}
