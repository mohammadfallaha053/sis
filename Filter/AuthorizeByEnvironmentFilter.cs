using LapisApi.App.Auth.Errors;
using LapisApi.Helpers;
using LapisApi.Helpers.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
namespace SisApi.Filter;

public class AuthorizeByEnvironmentFilter : IAuthorizationFilter
{
  private readonly IHostEnvironment _env;

  private readonly List<string> _excludedPaths = new()
  {
    "/api/auth/login",
    "/api/auth/register",
    "/api/auth/reset-password",
    "/api/auth/confirm-email",
    "/api/auth/send-otp",
    "/api/auth/verify-otp",
    "/api/region/get-all",
  };

  public AuthorizeByEnvironmentFilter(IHostEnvironment env)
  {
    _env = env;
  }

  public void OnAuthorization(AuthorizationFilterContext context)
  {
    // if (_env.IsDevelopment())
    //     return;

    var path = context.HttpContext.Request.Path.Value?.ToLower();
    if (_excludedPaths.Any(p => path != null && path.Contains(p)))
      return;

    var user = context.HttpContext.User;
    if (user?.Identity?.IsAuthenticated != true)
    {
      var acceptLanguage = context.HttpContext.Request.Headers["Accept-Language"].FirstOrDefault();
      var language = (acceptLanguage?.Contains("ar") ?? false) ? AppLanguageEnum.Ar : AppLanguageEnum.En;

      var error = ErrorDetails.FromError(AuthErrors.Unauthorized, language);

      context.Result = new JsonResult(new AppResponse<object>
        {
          IsSuccess = false,
          Error = error
        }
      )
      {
        StatusCode = StatusCodes.Status401Unauthorized
      };
    }
  }
}