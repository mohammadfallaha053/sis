using LapisApi.App.Auth.Interfaces;
using Microsoft.AspNetCore.Identity;
using SisApi.App.Users.Model;
using System.Security.Claims;
namespace SisApi.App.Auth.Services;

public class ClaimService : IClaimService
{
  private readonly IHttpContextAccessor _httpContextAccessor;
  private readonly UserManager<ApplicationUser> _userManager;
  private readonly RoleManager<IdentityRole> _roleManager;

  public ClaimService(
    IHttpContextAccessor httpContextAccessor,
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager
  )
  {
    _httpContextAccessor = httpContextAccessor;
    _userManager = userManager;
    _roleManager = roleManager;
  }

  public async Task<bool> IsAdminAsync()
  {
    var userId = GetUserId();
    return await HasRoleAsync(userId, "Admin");
  }
  
  public async Task<bool> IsClientAsync()
  {
    var userId = GetUserId();
    return await HasRoleAsync(userId, "Client");
  }


  public async Task<bool> HasRoleAsync(string userId, string role)
  {
    var user = await _userManager.FindByIdAsync(userId);
    if (user == null)
    {
      return false;
    }

    return await _userManager.IsInRoleAsync(user, role);
  }

  public string? GetUserId()
  {
    return _httpContextAccessor.HttpContext?.User.FindFirstValue("uid");
  }

  public string? GetCenterId()
  {
    var centerId = _httpContextAccessor.HttpContext?.User.FindFirstValue("centerId");
    return string.IsNullOrWhiteSpace(centerId) ? null : centerId;
  }

  public string? GetEmail() =>
    _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Email);

  public bool IsAuthenticated() =>
    _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;

  public IEnumerable<string> GetRoles()
  {
    return _httpContextAccessor.HttpContext?.User.FindAll("roles").Select(r => r.Value) ?? Enumerable.Empty<string>();
  }
}