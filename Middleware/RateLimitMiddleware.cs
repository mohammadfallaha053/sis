using System.Globalization;
using LapisApi.Helpers.Responses;
using LapisApi.Helpers.Security;
using LapisApi.App.Auth.Errors;

namespace LapisApi.Middleware;

public class RateLimitMiddleware : IMiddleware
{
  private readonly IAttemptTrackerService _tracker;

  public RateLimitMiddleware(IAttemptTrackerService tracker)
  {
    _tracker = tracker;
  }

  public async Task InvokeAsync(HttpContext context, RequestDelegate next)
  {
    var ip = context.Connection.RemoteIpAddress?.ToString();
    var path = context.Request.Path.ToString().ToLowerInvariant();

    if (string.IsNullOrWhiteSpace(ip))
    {
      await next(context);
      return;
    }

    var key = $"ratelimit:{ip}:{path}";

    var isLimited = await _tracker.IsLimitedAsync(
      key,
      maxAttempts: 10,
      window: TimeSpan.FromSeconds(10),
      lockoutDuration: TimeSpan.FromMinutes(1)
    );

    if (isLimited)
    {
      var lang = context.Request.Headers["Accept-Language"].ToString()?.ToLower() ?? "en";
      var error = AuthErrors.TooManyRequests;

      var response = new AppResponse<object>
      {
        IsSuccess = false,
        Error = new ErrorDetails
        {
          Code = error.Code,
          Message = error.GetLocalizedMessage(lang)
        }
      };

      context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
      context.Response.ContentType = "application/json";
      await context.Response.WriteAsJsonAsync(response);
      return;
    }

    await next(context);
  }
}