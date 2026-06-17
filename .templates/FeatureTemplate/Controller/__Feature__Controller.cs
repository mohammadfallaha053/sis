using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransfersApi.App.__Feature__s.Dto;
using TransfersApi.App.__Feature__s.Dto.Request.Commands;
using TransfersApi.App.__Feature__s.Dto.Request.Queries;
using TransfersApi.App.__Feature__s.Interfaces;
using TransfersApi.Interfaces.Auth;
namespace TransfersApi.App.__Feature__s.Controller;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/[controller]")]
public class __Feature__Controller : ControllerBase
{
  private readonly I__Feature__Service ___feature__Service;

  public __Feature__Controller(I__Feature__Service __Feature__Service)
  {
    ___feature__Service = __Feature__Service;
  }

  [HttpPost("add")]
  public async Task<IActionResult> Add([FromBody] __Feature__CreateCommand command)
  {
    var result = await ___feature__Service.AddAsync(command);

    return result.ToActionResult(this);
  }

  [HttpGet("get-all")]
  public async Task<IActionResult> GetAll([FromQuery] __Feature__GetAllQuery query)
  {
    var result = await ___feature__Service.GetAllAsync(query);
    return result.ToActionResult(this);
  }

  [HttpGet("get-by-id/{id}")]
  public async Task<IActionResult> GetById(int id)
  {
    var result = await ___feature__Service.GetByIdAsync(id);
    return result.ToActionResult(this);
  }

  [HttpPut("edit/{id}")]
  public async Task<IActionResult> Update(int id, [FromBody] __Feature__UpdateCommand command)
  {
    var result = await ___feature__Service.UpdateAsync(id, command);

    return result.ToActionResult(this);
  }

  [HttpDelete("delete/{id}")]
  public async Task<IActionResult> Delete(int id)
  {
    var result = await ___feature__Service.DeleteAsync(id);

    if (result.IsSuccess)
    {
      return NoContent();
    }

    return result.ToActionResult(this);
  }
}