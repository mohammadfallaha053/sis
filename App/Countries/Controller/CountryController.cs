using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LapisApi.App.Auth.Enums;
using LapisApi.App.Cities.Dto;
using LapisApi.App.Cities.Dto.Request.Commands;
using LapisApi.Filter;
using LapisApi.Interfaces.Cities;
namespace LapisApi.App.Cities.Controller;

[Authorize]
[ApiController]
[Route("api/City")]
public class CityController : ControllerBase
{
  private readonly ICityService _CityService;

  public CityController(ICityService CityService)
  {
    _CityService = CityService;
  }
  [Authorize(Roles = nameof(RoleEnum.Admin))]
  [HttpPost("add")]
  public async Task<IActionResult> AddCity([FromBody] CityCreateCommand dto)
  {
    var result = await _CityService.AddCityAsync(dto);

    return result.ToActionResult(this);
  }
    [ServiceFilter(typeof(ActiveUserAuthorizationFilter))]
    [Authorize(Roles = nameof(RoleEnum.Admin) + "," + nameof(RoleEnum.Client))]
    [HttpGet("get-all")]
  public async Task<IActionResult> GetAllCities([FromQuery] CityGetAllQuery CityGetAllQuery)
  {
    var result = await _CityService.GetAllCitiesAsync(CityGetAllQuery);
    return result.ToActionResult(this);
  }
  [Authorize]
  [HttpGet("get-auto-complete")]
  public async Task<IActionResult> GetAutoComplete([FromQuery] CityGetAutoCompleteQuery query)
  {
    var result = await _CityService.GetAutoComplete(query);
    return result.ToActionResult(this);
  }
  [Authorize(Roles = nameof(RoleEnum.Admin))]
  [HttpGet("get-by-id/{id}")]
  public async Task<IActionResult> GetCityById(int id)
  {

    var result = await _CityService.GetCityByIdAsync(id);
    return result.ToActionResult(this);
  }

  [Authorize(Roles = nameof(RoleEnum.Admin))]
  [HttpPut("edit/{id}")]
  public async Task<IActionResult> UpdateCity(int id, [FromBody] UpdateCityCommand CityCommand)
  {
    var result = await _CityService.UpdateCityAsync(id, CityCommand);

    return result.ToActionResult(this);
  }
  [Authorize(Roles = nameof(RoleEnum.Admin))]
  [HttpDelete("delete/{id}")]
  public async Task<IActionResult> DeleteCity(int id)
  {
    var result = await _CityService.DeleteCityAsync(id);

    if (result.IsSuccess)
    {
      return NoContent();
    }

    return result.ToActionResult(this);
  }
}