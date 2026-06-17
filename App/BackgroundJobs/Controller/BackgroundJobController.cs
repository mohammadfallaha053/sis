//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using LapisApi.App.Auth.Enums;
//using LapisApi.App.Auth.Interfaces;
//using LapisApi.App.BackgroundJobs.Dto;
//using LapisApi.App.BackgroundJobs.Dto.Request.Commands;
//using LapisApi.App.BackgroundJobs.Interfaces;
//using LapisApi.Interfaces.Auth;
//namespace LapisApi.App.BackgroundJobs.Controller;

//[ApiController]
//[Route("api/[controller]")]
//public class BackgroundJobController : ControllerBase
//{
//  private readonly IBackgroundJobService _backgroundJobService;
//  private IClaimService _claimService;

//  public BackgroundJobController(IBackgroundJobService BackgroundJobService, IClaimService claimsService)
//  {
//    _backgroundJobService = BackgroundJobService;
//    _claimService = claimsService;
//  }

//  //[HttpPost("add")]
//  //public async Task<IActionResult> Add([FromBody] BackgroundJobCreateRequest request)
//  //{
//  //  var result = await _backgroundJobService.AddAsync(request);
  
//  //  return result.ToActionResult(this);
//  //}
//  //[Authorize(Roles = nameof(RoleEnum.Admin))]
//  //[HttpGet("get-all")]
//  //public async Task<IActionResult> GetAll([FromQuery] BackgroundJobGetAllQuery query)
//  //{
//  //  var result = await _backgroundJobService.GetAllAsync(query);
//  //  return result.ToActionResult(this);
//  //}

//  // [HttpGet("get-by-id/{id}")]
//  // public async Task<IActionResult> GetById(string id)
//  // {
//  //   var result = await _backgroundJobService.GetByIdAsync(id);
//  //   return result.ToActionResult(this);
//  // }

//  // [HttpPut("edit/{id}")]
//  // public async Task<IActionResult> Update(string id, [FromBody] BackgroundJobUpdateRequest request)
//  // {
//  //   var result = await _backgroundJobService.UpdateAsync(id, request);
//  //
//  //   return result.ToActionResult(this);
//  // }
//  //[Authorize(Roles = nameof(RoleEnum.Admin))]
//  //[HttpDelete("delete/{id}")]
//  //public async Task<IActionResult> Delete(string id)
//  //{
//  //  var result = await _backgroundJobService.DeleteAsync(id);

//  //  if (result.IsSuccess)
//  //  {
//  //    return NoContent();
//  //  }

//  //  return result.ToActionResult(this);
//  //}
//}