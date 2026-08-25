using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SisApi.App.Products.Dto.Request.Commands;
using SisApi.App.Products.Dto.Request.Queries;
using SisApi.App.Products.Interfaces;

namespace SisApi.App.Products.Controller;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
  private readonly IProductsService _productsService;

  public ProductsController(IProductsService productsService)
  {
    _productsService = productsService;
  }

  [Authorize(Roles = "Admin")]
  [HttpPost("add")]
  public async Task<IActionResult> Add(
    [FromBody] ProductsCreateCommand command
  )
  {
    var result = await _productsService.AddAsync(command);
    return result.ToActionResult(this);
  }

  [HttpGet("get-all")]
  public async Task<IActionResult> GetAll(
    [FromQuery] ProductsGetAllQuery query
  )
  {
    var result = await _productsService.GetAllAsync(query);
    return result.ToActionResult(this);
  }

  [HttpGet("get-by-id/{id}")]
  public async Task<IActionResult> GetById(int id)
  {
    var result = await _productsService.GetByIdAsync(id);
    return result.ToActionResult(this);
  }

  [Authorize(Roles = "Admin")]
  [HttpPut("edit/{id}")]
  public async Task<IActionResult> Update(
    int id,
    [FromBody] ProductsUpdateCommand command
  )
  {
    var result = await _productsService.UpdateAsync(id, command);
    return result.ToActionResult(this);
  }

  [Authorize(Roles = "Client")]
  [HttpPost("{id}/purchase")]
  public async Task<IActionResult> Purchase(
    int id,
    [FromBody] ProductPurchaseCommand command
  )
  {
    var result = await _productsService.PurchaseAsync(id, command);
    return result.ToActionResult(this);
  }

  [Authorize(Roles = "Admin")]
  [HttpDelete("delete/{id}")]
  public async Task<IActionResult> Delete(int id)
  {
    var result = await _productsService.DeleteAsync(id);

    if (result.IsSuccess)
    {
      return NoContent();
    }

    return result.ToActionResult(this);
  }
}
