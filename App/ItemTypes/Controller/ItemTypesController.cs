using LapisApi.App.ItemTypes.Dto.Request.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SisApi.App.ItemTypes.Dto.Request.Commands;
using SisApi.App.ItemTypes.Dto.Request.Queries;
using SisApi.App.ItemTypes.Interfaces;
namespace SisApi.App.ItemTypes.Controller;


[ApiController]
[Route("api/[controller]")]
public class ItemTypesController : ControllerBase
{
  private readonly IItemTypesService _ItemTypesService;

  public ItemTypesController(IItemTypesService ItemTypesService)
  {
    _ItemTypesService = ItemTypesService;
  }
  [Authorize(Roles = "Admin")]
  [HttpPost("add")]
  public async Task<IActionResult> Add([FromBody] ItemTypesCreateCommand command)
  {
    var result = await _ItemTypesService.AddAsync(command);

    return result.ToActionResult(this);
  }

  [HttpGet("get-all")]
  public async Task<IActionResult> GetAll([FromQuery] ItemTypesGetAllQuery query)
  {
    var result = await _ItemTypesService.GetAllAsync(query);
    return result.ToActionResult(this);
  }

  [HttpGet("get-by-id/{id}")]
  public async Task<IActionResult> GetById(int id)
  {
    var result = await _ItemTypesService.GetByIdAsync(id);
    return result.ToActionResult(this);
  }
  [Authorize(Roles = "Admin")]
  [HttpPut("edit/{id}")]
  public async Task<IActionResult> Update(int id, [FromBody] ItemTypesUpdateCommand command)
  {
    var result = await _ItemTypesService.UpdateAsync(id, command);

    return result.ToActionResult(this);
  }
  [Authorize(Roles = "Admin")]
  [HttpDelete("delete/{id}")]
  public async Task<IActionResult> Delete(int id)
  {
    var result = await _ItemTypesService.DeleteAsync(id);

    if (result.IsSuccess)
    {
      return NoContent();
    }

    return result.ToActionResult(this);
  }
}