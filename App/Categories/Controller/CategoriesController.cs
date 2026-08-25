using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SisApi.App.Categories.Dto.Request.Commands;
using SisApi.App.Categories.Dto.Request.Queries;
using SisApi.App.Categories.Interfaces;

namespace SisApi.App.Categories.Controller;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
  private readonly ICategoriesService _categoriesService;

  public CategoriesController(ICategoriesService categoriesService)
  {
    _categoriesService = categoriesService;
  }

  [Authorize(Roles = "Admin")]
  [HttpPost("add")]
  public async Task<IActionResult> Add(
    [FromBody] CategoriesCreateCommand command
  )
  {
    var result = await _categoriesService.AddAsync(command);
    return result.ToActionResult(this);
  }

  [HttpGet("get-all")]
  public async Task<IActionResult> GetAll(
    [FromQuery] CategoriesGetAllQuery query
  )
  {
    var result = await _categoriesService.GetAllAsync(query);
    return result.ToActionResult(this);
  }

  [HttpGet("get-by-id/{id}")]
  public async Task<IActionResult> GetById(int id)
  {
    var result = await _categoriesService.GetByIdAsync(id);
    return result.ToActionResult(this);
  }

  [Authorize(Roles = "Admin")]
  [HttpPut("edit/{id}")]
  public async Task<IActionResult> Update(
    int id,
    [FromBody] CategoriesUpdateCommand command
  )
  {
    var result = await _categoriesService.UpdateAsync(id, command);
    return result.ToActionResult(this);
  }

  [Authorize(Roles = "Admin")]
  [HttpDelete("delete/{id}")]
  public async Task<IActionResult> Delete(int id)
  {
    var result = await _categoriesService.DeleteAsync(id);

    if (result.IsSuccess)
    {
      return NoContent();
    }

    return result.ToActionResult(this);
  }
}