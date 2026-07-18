using LapisApi.App.Settings.Interfaces;
using Microsoft.AspNetCore.Mvc;
namespace SisApi.App.Users.Controller
{
  [Route("api/[controller]")]
  [ApiController]
  public class ClientController : ControllerBase
  {
    private readonly ISettingService _settingService;
    
    public ClientController(
      ISettingService settingService
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