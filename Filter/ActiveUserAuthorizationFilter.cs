using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using LapisApi.App.Auth.Errors;
using LapisApi.Helpers.Responses;
using LapisApi.App.Users.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using LapisApi.App.Auth.Enums;
using LapisApi.App.Users.Errors;
using LapisApi.Helpers;

namespace LapisApi.Filter;

public class ActiveUserAuthorizationFilter : IAsyncAuthorizationFilter
{
  private readonly UserManager<ApplicationUser> _userManager;

  public ActiveUserAuthorizationFilter(UserManager<ApplicationUser> userManager)
  {
    _userManager = userManager;
  }

  public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
  {
    var user = context.HttpContext.User;

    if (user?.Identity?.IsAuthenticated != true)
      return;

    var email = _userManager.GetUserId(user);
    var dbUser = await _userManager.Users
      .Include(u => u.Center)
      .FirstOrDefaultAsync(u => u.Email == email);

    if (dbUser == null || !dbUser.IsActive)
    {
      context.Result = new JsonResult(new AppResponse<object>
        {
          IsSuccess = false,
          Error = ErrorDetails.FromError(UserErrors.AccountDisabled, GetLanguage(context))
        }
      )
      {
        StatusCode = StatusCodes.Status401Unauthorized
      };
    }
  }

  private AppLanguageEnum GetLanguage(AuthorizationFilterContext context)
  {
    var lang = context.HttpContext.Request.Headers["Accept-Language"].FirstOrDefault();
    return (lang?.ToLower().Contains("ar") ?? false) ? AppLanguageEnum.Ar : AppLanguageEnum.En;
  }
}