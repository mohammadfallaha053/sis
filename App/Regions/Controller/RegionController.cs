
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SisApi.App.Auth.Enums;
using SisApi.App.Regions.Dto.Request.Commands;
using SisApi.App.Regions.Dto.Request.Queries;
using SisApi.App.Regions.Interfaces;
namespace SisApi.App.Regions.Controller;

[ApiController]
[Route("api/[controller]")]
public class RegionController : ControllerBase
{
  private readonly IRegionService _RegionService;

  public RegionController(IRegionService RegionService)
  {
    _RegionService = RegionService;
  }
  [Authorize(Roles = nameof(RoleEnum.Admin))]
  [HttpPost("add")]
  public async Task<IActionResult> AddRegion([FromBody] RegionCreateCommand command)
  {
    var result = await _RegionService.AddAsync(command);

    return result.ToActionResult(this);
  }

  [HttpGet("get-all")]
  public async Task<IActionResult> GetAll([FromQuery] RegionGetAllQuery query)
  {
    var result = await _RegionService.GetAllAsync(query);
    return result.ToActionResult(this);
  }

  [Authorize(Roles = nameof(RoleEnum.Admin))]
  [HttpGet("get-by-id/{id}")]
  public async Task<IActionResult> GetById(int id)
  {
    var result = await _RegionService.GetByIdAsync(id);
    return result.ToActionResult(this);
  }
  
  [Authorize(Roles = nameof(RoleEnum.Admin))]
  [HttpPut("edit/{id}")]
  public async Task<IActionResult> Update(int id, [FromBody] RegionUpdateCommand command)
  {
    var result = await _RegionService.UpdateAsync(id, command);

    return result.ToActionResult(this);
  }
  
  [Authorize(Roles = nameof(RoleEnum.Admin))]
  [HttpDelete("delete/{id}")]
  public async Task<IActionResult> Delete(int id)
  {
    var result = await _RegionService.DeleteAsync(id);

    if (result.IsSuccess)
    {
      return NoContent();
    }

    return result.ToActionResult(this);
  }
}