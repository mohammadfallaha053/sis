using System.Security.Claims;
namespace LapisApi.Middleware;

public class BypassAuthMiddleware
{
  private readonly RequestDelegate _next;
  private readonly IHostEnvironment _env;

  public BypassAuthMiddleware(RequestDelegate next, IHostEnvironment env)
  {
    _next = next;
    _env = env;
  }

  public async Task InvokeAsync(HttpContext context)
  {
    if (_env.IsDevelopment())
    {
      var identity = new ClaimsIdentity("Dev");
      identity.AddClaim(new Claim(ClaimTypes.Name, "DevUser"));
      identity.AddClaim(new Claim("uid", "82fc8e92-a89b-4833-bc45-fc451b811326"));
      identity.AddClaim(new Claim(ClaimTypes.Email, "Dev@Dev.com"));
      identity.AddClaim(new Claim(ClaimTypes.Role, "Admin"));

      var principal = new ClaimsPrincipal(identity);
      context.User = principal;
    }

    await _next(context);
  }
}
