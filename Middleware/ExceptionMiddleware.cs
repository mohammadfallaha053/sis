using JWT53.MyEnum;
using LapisApi.Helpers;
using LapisApi.Helpers.Responses;
using LapisApi.Shared;
using LapisApi.Shared.Errors;

public class ExceptionMiddleware
{
  private readonly RequestDelegate _next;
  private readonly ILogger<ExceptionMiddleware> _logger;
  private readonly IWebHostEnvironment _env;

  public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IWebHostEnvironment env)
  {
    _next = next;
    _logger = logger;
    _env = env;
  }

  public async Task Invoke(HttpContext context)
  {
    try
    {
      await _next(context);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
      context.Response.ContentType = "application/json";
      context.Response.StatusCode = StatusCodes.Status500InternalServerError;
      var acceptLanguage = context.Request.Headers["Accept-Language"].ToString();
      var language = acceptLanguage.Contains("ar") ? AppLanguageEnum.Ar : AppLanguageEnum.En;
      var appError = ErrorDetails.FromError(SharedErrors.ServerError, language);

      if (_env.IsDevelopment())
      {
        appError.Details = ex.Message;
      }

      var response = new AppResponse<string>
      {
        IsSuccess = false,
        Error = appError
      };

      await context.Response.WriteAsJsonAsync(response);
    }
  }
}