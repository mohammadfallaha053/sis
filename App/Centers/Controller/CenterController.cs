using LapisApi.App.Auth.Enums;
using LapisApi.App.Auth.Interfaces;
using LapisApi.App.Centers.Dto;
using LapisApi.App.Centers.Dto.Request.Commands;
using LapisApi.App.Centers.Interfaces;
using LapisApi.Filter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace SisApi.App.Centers.Controller;

[ApiController]
[Route("api/[controller]")]
public class CenterController : ControllerBase
{

  private readonly ICenterService _centerService;
  private IClaimService _claimService;

  public CenterController(ICenterService centerService, IClaimService claimsService)
  {
    _centerService = centerService;
    _claimService = claimsService;
  }
  
  [Authorize(Roles = nameof(RoleEnum.Admin))]
  [HttpPost("add")]
  public async Task<IActionResult> AddCenter([FromBody] CenterCreateCommand command)
  {
    var result = await _centerService.AddCenterAsync(command);

    return result.ToActionResult(this);
  }
  [Authorize(Roles = nameof(RoleEnum.Admin))]
  [HttpGet("get-all")]
  public async Task<IActionResult> GetAllCenters([FromQuery] CenterGetAllQuery query)
  {
    var result = await _centerService.GetAllCentersAsync(query);
    return result.ToActionResult(this);
  }
  [Authorize(Roles = nameof(RoleEnum.Admin))]
  [HttpGet("get-by-id/{id}")]
  public async Task<IActionResult> GetCenterById(string id)
  {
    var result = await _centerService.GetCenterByIdAsync(id);
    return result.ToActionResult(this);
  }
  
  [ServiceFilter(typeof(ActiveUserAuthorizationFilter))]
  [Authorize(Roles = nameof(RoleEnum.Client))]
  [HttpGet("get-center-info")]
  public async Task<IActionResult> GetCenterInfo()
  {
    var result = await _centerService.GetCenterInfo();
    return result.ToActionResult(this);
  }

  [Authorize(Roles = nameof(RoleEnum.Client))]
  [HttpPut("update-center-info")]
  public async Task<IActionResult> UpdateCenterInfo([FromBody] CenterUpdateInfoCommand command)
  {
    var result = await _centerService.UpdateCenterInfoAsync(command);
    return result.ToActionResult(this);
  }

  [Authorize(Roles = nameof(RoleEnum.Admin))]
  [HttpPut("edit/{id}")]
  public async Task<IActionResult> UpdateCenter(string id, [FromBody] CenterUpdateCommand request)
  {
    var result = await _centerService.UpdateCenterAsync(id, request);

    return result.ToActionResult(this);
  }

  // [Authorize(Roles = nameof(RoleEnum.Admin))]
  // [HttpDelete("delete/{id}")]
  // public async Task<IActionResult> DeleteCenter(string id)
  // {
  //   var result = await _centerService.DeleteCenterAsync(id);
  //
  //   if (result.IsSuccess)
  //   {
  //     return NoContent();
  //   }
  //
  //   return result.ToActionResult(this);
  // }
}