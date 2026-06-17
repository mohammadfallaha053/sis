namespace LapisApi.Middleware;

public class BotProtectionMiddleware : IMiddleware
{
  public async Task InvokeAsync(HttpContext context, RequestDelegate next)
  {
    var userAgent = context.Request.Headers["User-Agent"].ToString();
    if (string.IsNullOrWhiteSpace(userAgent))
    {
      context.Response.StatusCode = StatusCodes.Status403Forbidden;
      await context.Response.WriteAsync("Missing User-Agent header.");
      return;
    }

    await next(context);
  }
}