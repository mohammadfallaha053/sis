// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;
// using LapisApi.App.Settings.Dto;
// using LapisApi.App.Settings.Dto.Commands;
// using LapisApi.App.Settings.Interfaces;
// using LapisApi.Interfaces.Cities;
// namespace LapisApi.App.Settings.Controller;
//
// [Authorize]
// [ApiController]
// [Route("api/[controller]")]
// public class AdsController : ControllerBase
// {
//   private readonly ISettingService _SettingService;
//
//   public AdsController(ISettingService SettingService)
//   {
//     _SettingService = SettingService;
//   }
//   
//
//   [HttpGet("get")]
//   public async Task<IActionResult> GetAdsAsync()
//   {
//     var result = await _SettingService.GetAdsAsync();
//     return result.ToActionResult(this);
//   }
//   
//
//   [HttpPut("edit")]
//   public async Task<IActionResult> UpdateAds([FromBody] AdsUpdateCommand command)
//   {
//     var result = await _SettingService.UpdateAdsAsync(command);
//
//     return result.ToActionResult(this);
//   }
//   
// }