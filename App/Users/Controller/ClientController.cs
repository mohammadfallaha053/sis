using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LapisApi.App.Auth.Dto;
using LapisApi.App.Auth.Enums;
using LapisApi.App.Centers.Interfaces;
using LapisApi.App.Regions.Dto;
using LapisApi.App.Regions.Interfaces;
using LapisApi.App.Comments.Dto.Request.Commands;
using LapisApi.App.Comments.Interfaces;
using LapisApi.App.Cities.Dto;
using LapisApi.App.Settings.Interfaces;
using LapisApi.Filter;
using LapisApi.Interfaces.Auth;
using LapisApi.Interfaces.Cities;
using SisApi.App.Regions.Interfaces;
namespace LapisApi.App.Users.Controller
{

  [Route("api/[controller]")]
  [ApiController]
  public class ClientController : ControllerBase
  {
    private readonly ISettingService _settingService;
    
    public ClientController(
      IAuthService authService,
      ICommentService commentService,
      ISettingService settingService,
      ICenterService centerService,
      IRegionService RegionService,
      ICityService CityService
    )
    {
      _settingService = settingService;
    }
    
    // [HttpGet("get-slider-comments")]
    // public async Task<IActionResult> GetSliderComments()
    // {
    //   var result = await _commentService.GetSlider();
    //
    //   return result.ToActionResult(this);
    // }

    [HttpGet("get-ads")]
    public async Task<IActionResult> GetClientAds()
    {
      var result = await _settingService.GetClientAdsAsync();

      return result.ToActionResult(this);
    }
    

    [HttpGet("get-settings")]
    public async Task<IActionResult> GetSettingsAsync()
    {
      var result = await _settingService.GetSettingsAsync();
      return result.ToActionResult(this);
    }
  }
}