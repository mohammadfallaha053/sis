using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SisApi.App.Statistics.Dto.Request.Queries;
using SisApi.App.Statistics.Interfaces;

namespace SisApi.App.Statistics.Controller;

[ApiController]
[Route("api/[controller]")]
public class StatisticsController : ControllerBase
{
  private readonly IStatisticsService _statisticsService;

  public StatisticsController(IStatisticsService statisticsService)
  {
    _statisticsService = statisticsService;
  }

  // Same endpoint for both roles.
  // The service returns a role-aware response:
  // Admin   => Common + Admin
  // Manager => Common + Manager
  [Authorize(Roles = "Admin,Manager")]
  [HttpGet("get")]
  public async Task<IActionResult> Get([FromQuery] StatisticsGetQuery query)
  {
    var result = await _statisticsService.GetAsync(query);
    return result.ToActionResult(this);
  }
}
