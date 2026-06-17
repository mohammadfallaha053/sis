using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using LapisApi.App.Auth.Enums;
using LapisApi.App.Auth.Interfaces;
using LapisApi.App.BackgroundJobs.Interfaces;
using LapisApi.App.Users.Dto;
using LapisApi.App.Users.Dto.Request.Commands;
using LapisApi.App.Users.Dto.Request.Queries;
using LapisApi.App.Users.Interfaces;
using LapisApi.App.Users.Model;
using LapisApi.Filter;
namespace LapisApi.App.Users.Controller;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
  private readonly IUserService _userService;
  private readonly IClaimService _claimService;
  private readonly IBackgroundJobService _backgroundJobService;

  public UserController(
    IUserService userService,
    UserManager<ApplicationUser> userManager,
    IClaimService claimService,
    IBackgroundJobService backgroundJobService
  )
  {
    _userService = userService;
    _claimService = claimService;
    _backgroundJobService = backgroundJobService;
  }

  // [Authorize(Roles = "Admin")]
  // [HttpDelete("admin/delete-user/{id}")]
  // public async Task<IActionResult> DeleteUser(string id)
  // {
  //   var result = await _userService.DeleteUserAsync(id);
  //   if (result.Succeeded)
  //     return Ok();
  //
  //   return BadRequest(result.Errors);
  // }
  [Authorize(Roles = nameof(RoleEnum.Admin))]
  [HttpGet("get-all")]
  public async Task<IActionResult> GetAllUsers([FromQuery] UserGetAllQuery getAllQuery)
  {
    var result = await _userService.GetAllUsersAsync(getAllQuery);
    return result.ToActionResult(this);
  }
  [Authorize(Roles = nameof(RoleEnum.Admin))]
  [HttpGet("get-by-id/{id}")]
  public async Task<IActionResult> GetUserById(string id)
  {
    var result = await _userService.GetUserByIdAsync(id);
    return result.ToActionResult(this);
  }

  [ServiceFilter(typeof(ActiveUserAuthorizationFilter))]
  [Authorize]
  [HttpPost("change-password")]
  public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest model)
  {
    var result = await _userService.ChangePasswordAsync(model);

    return result.ToActionResult(this);
  }
  
  [ServiceFilter(typeof(ActiveUserAuthorizationFilter))]
  [Authorize]
  [HttpPut("edit-profile")]
  public async Task<IActionResult> UpdateCurrentUser([FromBody] UpdateUserRequest request)
  {
    var result = await _userService.UpdateUserAsync(request);
    return result.ToActionResult(this);
  }
  
  [ServiceFilter(typeof(ActiveUserAuthorizationFilter))]
  [Authorize]
  [HttpGet("get-info")]
  public async Task<IActionResult> getInfoCurrentUser()
  {
    var userId = _claimService.GetUserId();
    var result = await _userService.GetUserByIdAsync(userId);

    return result.ToActionResult(this);
  }
  
  [Authorize(Roles = nameof(RoleEnum.Admin))]
  [HttpPost("toggle-disable")]
  public async Task<IActionResult> DisableUser([FromBody] DisableAgentRequest request)
  {
    var result = await _userService.DisableUserAsync(request);
    return result.ToActionResult(this);
  }
}