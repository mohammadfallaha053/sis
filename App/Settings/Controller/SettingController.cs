using LapisApi.App.Auth.Enums;
using LapisApi.App.Settings.Dto.Commands;
using LapisApi.App.Settings.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace SisApi.App.Settings.Controller;

[Authorize(Roles = nameof(RoleEnum.Admin))]
[ApiController]
[Route("api/[controller]")]
public class SettingController : ControllerBase
{
  private readonly ISettingService _SettingService;

  public SettingController(ISettingService SettingService)
  {
    _SettingService = SettingService;
  }
  

  [HttpGet("get")]
  public async Task<IActionResult> GetSettingsAsync()
  {
    var result = await _SettingService.GetSettingsAsync();
    return result.ToActionResult(this);
  }
  
  [HttpGet("get-statistic")]
  public async Task<IActionResult> GetStatistic()
  {
    var result = await _SettingService.GetGetStatisticAsync();
    return result.ToActionResult(this);
  }
  

  [HttpPut("edit")]
  public async Task<IActionResult> UpdateSetting([FromBody] SettingUpdateCommand settingCommand)
  {
    var result = await _SettingService.UpdateSettingAsync(settingCommand);

    return result.ToActionResult(this);
  }
  
}