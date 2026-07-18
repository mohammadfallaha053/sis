using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SisApi.App.Centers.Dto.Request.Commands;
using SisApi.App.Centers.Dto.Request.Queries;
using SisApi.App.Centers.Interfaces;
namespace LapisApi.App.Centers.Controller;


[ApiController]
[Route("api/[controller]")]
public class CentersController : ControllerBase
{
  private readonly ICentersService _CentersService;

  public CentersController(ICentersService CentersService)
  {
    _CentersService = CentersService;
  }
  [Authorize(Roles = "Admin")]
  [HttpPost("add")]
  public async Task<IActionResult> Add([FromBody] CentersCreateCommand command)
  {
    var result = await _CentersService.AddAsync(command);

    return result.ToActionResult(this);
  }

  [HttpGet("get-all")]
  public async Task<IActionResult> GetAll([FromQuery] CentersGetAllQuery query)
  {
    var result = await _CentersService.GetAllAsync(query);
    return result.ToActionResult(this);
  }

  [HttpGet("get-by-id/{id}")]
  public async Task<IActionResult> GetById(int id)
  {
    var result = await _CentersService.GetByIdAsync(id);
    return result.ToActionResult(this);
  }
  [Authorize(Roles = "Admin")]
  [HttpPut("edit/{id}")]
  public async Task<IActionResult> Update(int id, [FromBody] CentersUpdateCommand command)
  {
    var result = await _CentersService.UpdateAsync(id, command);

    return result.ToActionResult(this);
  }
  [Authorize(Roles = "Admin")]
  [HttpDelete("delete/{id}")]
  public async Task<IActionResult> Delete(int id)
  {
    var result = await _CentersService.DeleteAsync(id);

    if (result.IsSuccess)
    {
      return NoContent();
    }

    return result.ToActionResult(this);
  }
}