using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SisApi.App.PointsTransactions.Dto.Request.Queries;
using SisApi.App.PointsTransactions.Interfaces;

namespace SisApi.App.PointsTransactions.Controller;

[ApiController]
[Route("api/[controller]")]
public class PointsTransactionsController : ControllerBase
{
  private readonly IPointsTransactionsService _pointsTransactionsService;

  public PointsTransactionsController(
    IPointsTransactionsService pointsTransactionsService
  )
  {
    _pointsTransactionsService = pointsTransactionsService;
  }

  [Authorize(Roles = "Admin,Client")]
  [HttpGet("get-all")]
  public async Task<IActionResult> GetAll(
    [FromQuery] PointsTransactionsGetAllQuery query
  )
  {
    var result =
      await _pointsTransactionsService.GetAllAsync(query);

    return result.ToActionResult(this);
  }
}
